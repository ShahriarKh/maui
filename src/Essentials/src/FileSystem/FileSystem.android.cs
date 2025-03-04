using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Android.Provider;
using Android.Webkit;
using AndroidUri = Android.Net.Uri;

namespace Microsoft.Maui.Essentials.Implementations
{
	public partial class FileSystemImplementation : IFileSystem, IPlatformFileSystem
	{
		internal const string EssentialsFolderHash = "2203693cc04e0be7f4f024d5f9499e13";

		const string storageTypePrimary = "primary";
		const string storageTypeRaw = "raw";
		const string storageTypeImage = "image";
		const string storageTypeVideo = "video";
		const string storageTypeAudio = "audio";
		static readonly string[] contentUriPrefixes =
		{
			"content://downloads/public_downloads",
			"content://downloads/my_downloads",
			"content://downloads/all_downloads",
		};

		internal const string UriSchemeFile = "file";
		internal const string UriSchemeContent = "content";

		internal const string UriAuthorityExternalStorage = "com.android.externalstorage.documents";
		internal const string UriAuthorityDownloads = "com.android.providers.downloads.documents";
		internal const string UriAuthorityMedia = "com.android.providers.media.documents";

		string PlatformCacheDirectory
			=> Platform.AppContext.CacheDir.AbsolutePath;

		string PlatformAppDataDirectory
			=> Platform.AppContext.FilesDir.AbsolutePath;

		Task<Stream> PlatformOpenAppPackageFileAsync(string filename) =>
			Task.FromResult(PlatformOpenAppPackageFile(filename));

		Task<bool> PlatformAppPackageFileExistsAsync(string filename)
		{
			try
			{
				using var stream = PlatformOpenAppPackageFile(filename);
				return Task.FromResult(true);
			}
			catch (FileNotFoundException)
			{
				return Task.FromResult(false);
			}
		}

		Stream PlatformOpenAppPackageFile(string filename)
		{
			if (filename == null)
				throw new ArgumentNullException(nameof(filename));

			filename = NormalizePath(filename);

			try
			{
				return Platform.AppContext.Assets.Open(filename);
			}
			catch (Java.IO.FileNotFoundException ex)
			{
				throw new FileNotFoundException(ex.Message, filename, ex);
			}
		}

		static string NormalizePath(string filename) =>
			filename.Replace('\\', Path.DirectorySeparatorChar);

		public Java.IO.File GetTemporaryFile(Java.IO.File root, string fileName)
		{
			// create the directory for all Essentials files
			var rootDir = new Java.IO.File(root, EssentialsFolderHash);
			rootDir.Mkdirs();
			rootDir.DeleteOnExit();

			// create a unique directory just in case there are multiple file with the same name
			var tmpDir = new Java.IO.File(rootDir, Guid.NewGuid().ToString("N"));
			tmpDir.Mkdirs();
			tmpDir.DeleteOnExit();

			// create the new temporary file
			var tmpFile = new Java.IO.File(tmpDir, fileName);
			tmpFile.DeleteOnExit();

			return tmpFile;
		}


		public string EnsurePhysicalPath(AndroidUri uri, bool requireExtendedAccess = true)
		{
			// if this is a file, use that
			if (uri.Scheme.Equals(UriSchemeFile, StringComparison.OrdinalIgnoreCase))
				return uri.Path;

			// try resolve using the content provider
			var absolute = ResolvePhysicalPath(uri, requireExtendedAccess);
			if (!string.IsNullOrWhiteSpace(absolute) && Path.IsPathRooted(absolute))
				return absolute;

			// fall back to just copying it
			var cached = CacheContentFile(uri);
			if (!string.IsNullOrWhiteSpace(cached) && Path.IsPathRooted(cached))
				return cached;

			throw new FileNotFoundException($"Unable to resolve absolute path or retrieve contents of URI '{uri}'.");
		}

		static string ResolvePhysicalPath(AndroidUri uri, bool requireExtendedAccess = true)
		{
			if (uri.Scheme.Equals(UriSchemeFile, StringComparison.OrdinalIgnoreCase))
			{
				// if it is a file, then return directly

				var resolved = uri.Path;
				if (File.Exists(resolved))
					return resolved;
			}
			else if (!requireExtendedAccess || !OperatingSystem.IsAndroidVersionAtLeast(29))
			{
				// if this is on an older OS version, or we just need it now

				if (Platform.HasApiLevelKitKat && DocumentsContract.IsDocumentUri(Platform.AppContext, uri))
				{
					var resolved = ResolveDocumentPath(uri);
					if (File.Exists(resolved))
						return resolved;
				}
				else if (uri.Scheme.Equals(UriSchemeContent, StringComparison.OrdinalIgnoreCase))
				{
					var resolved = ResolveContentPath(uri);
					if (File.Exists(resolved))
						return resolved;
				}
			}

			return null;
		}

		static string ResolveDocumentPath(AndroidUri uri)
		{
			Debug.WriteLine($"Trying to resolve document URI: '{uri}'");

			var docId = DocumentsContract.GetDocumentId(uri);

			var docIdParts = docId?.Split(':');
			if (docIdParts == null || docIdParts.Length == 0)
				return null;

			if (uri.Authority.Equals(UriAuthorityExternalStorage, StringComparison.OrdinalIgnoreCase))
			{
				Debug.WriteLine($"Resolving external storage URI: '{uri}'");

				if (docIdParts.Length == 2)
				{
					var storageType = docIdParts[0];
					var uriPath = docIdParts[1];

					// This is the internal "external" memory, NOT the SD Card
					if (storageType.Equals(storageTypePrimary, StringComparison.OrdinalIgnoreCase))
					{
#pragma warning disable CS0618 // Type or member is obsolete
						var root = global::Android.OS.Environment.ExternalStorageDirectory.Path;
#pragma warning restore CS0618 // Type or member is obsolete

						return Path.Combine(root, uriPath);
					}

					// TODO: support other types, such as actual SD Cards
				}
			}
			else if (uri.Authority.Equals(UriAuthorityDownloads, StringComparison.OrdinalIgnoreCase))
			{
				Debug.WriteLine($"Resolving downloads URI: '{uri}'");

				// NOTE: This only really applies to older Android vesions since the privacy changes

				if (docIdParts.Length == 2)
				{
					var storageType = docIdParts[0];
					var uriPath = docIdParts[1];

					if (storageType.Equals(storageTypeRaw, StringComparison.OrdinalIgnoreCase))
						return uriPath;
				}

				// ID could be "###" or "msf:###"
				var fileId = docIdParts.Length == 2
					? docIdParts[1]
					: docIdParts[0];

				foreach (var prefix in contentUriPrefixes)
				{
					var uriString = prefix + "/" + fileId;
					var contentUri = AndroidUri.Parse(uriString);

					if (GetDataFilePath(contentUri) is string filePath)
						return filePath;
				}
			}
			else if (uri.Authority.Equals(UriAuthorityMedia, StringComparison.OrdinalIgnoreCase))
			{
				Debug.WriteLine($"Resolving media URI: '{uri}'");

				if (docIdParts.Length == 2)
				{
					var storageType = docIdParts[0];
					var uriPath = docIdParts[1];

					AndroidUri contentUri = null;
					if (storageType.Equals(storageTypeImage, StringComparison.OrdinalIgnoreCase))
						contentUri = MediaStore.Images.Media.ExternalContentUri;
					else if (storageType.Equals(storageTypeVideo, StringComparison.OrdinalIgnoreCase))
						contentUri = MediaStore.Video.Media.ExternalContentUri;
					else if (storageType.Equals(storageTypeAudio, StringComparison.OrdinalIgnoreCase))
						contentUri = MediaStore.Audio.Media.ExternalContentUri;
#pragma warning disable CS0618
					if (contentUri != null && GetDataFilePath(contentUri, $"{MediaStore.MediaColumns.Id}=?", new[] { uriPath }) is string filePath)
						return filePath;
#pragma warning restore CS0618
				}
			}

			Debug.WriteLine($"Unable to resolve document URI: '{uri}'");

			return null;
		}

		static string ResolveContentPath(AndroidUri uri)
		{
			Debug.WriteLine($"Trying to resolve content URI: '{uri}'");

			if (GetDataFilePath(uri) is string filePath)
				return filePath;

			// TODO: support some additional things, like Google Photos if that is possible

			Debug.WriteLine($"Unable to resolve content URI: '{uri}'");

			return null;
		}

		string CacheContentFile(AndroidUri uri)
		{
			if (!uri.Scheme.Equals(UriSchemeContent, StringComparison.OrdinalIgnoreCase))
				return null;

			Debug.WriteLine($"Copying content URI to local cache: '{uri}'");

			// open the source stream
			using var srcStream = OpenContentStream(uri, out var extension);
			if (srcStream == null)
				return null;

			// resolve or generate a valid destination path
#pragma warning disable CS0618
			var filename = GetColumnValue(uri, MediaStore.Files.FileColumns.DisplayName) ?? Guid.NewGuid().ToString("N");
#pragma warning restore CS0618

			if (!Path.HasExtension(filename) && !string.IsNullOrEmpty(extension))
				filename = Path.ChangeExtension(filename, extension);

			// create a temporary file
			var hasPermission = Permissions.IsDeclaredInManifest(global::Android.Manifest.Permission.WriteExternalStorage);
			var root = hasPermission
				? Platform.AppContext.ExternalCacheDir
				: Platform.AppContext.CacheDir;
			var tmpFile = GetTemporaryFile(root, filename);

			// copy to the destination
			using var dstStream = File.Create(tmpFile.CanonicalPath);
			srcStream.CopyTo(dstStream);

			return tmpFile.CanonicalPath;
		}

		static Stream OpenContentStream(AndroidUri uri, out string extension)
		{
			var isVirtual = IsVirtualFile(uri);
			if (isVirtual)
			{
				Debug.WriteLine($"Content URI was virtual: '{uri}'");
				return GetVirtualFileStream(uri, out extension);
			}

			extension = GetFileExtension(uri);
			return Platform.ContentResolver.OpenInputStream(uri);
		}

		static bool IsVirtualFile(AndroidUri uri)
		{
			if (!DocumentsContract.IsDocumentUri(Platform.AppContext, uri))
				return false;

			var value = GetColumnValue(uri, DocumentsContract.Document.ColumnFlags);
			if (!string.IsNullOrEmpty(value) && int.TryParse(value, out var flagsInt))
			{
				var flags = (DocumentContractFlags)flagsInt;
				return flags.HasFlag(DocumentContractFlags.VirtualDocument);
			}

			return false;
		}

		static Stream GetVirtualFileStream(AndroidUri uri, out string extension)
		{
			var mimeTypes = Platform.ContentResolver.GetStreamTypes(uri, FileSystem.MimeTypes.All);
			if (mimeTypes?.Length >= 1)
			{
				var mimeType = mimeTypes[0];

				var stream = Platform.ContentResolver
					.OpenTypedAssetFileDescriptor(uri, mimeType, null)
					.CreateInputStream();

				extension = MimeTypeMap.Singleton.GetExtensionFromMimeType(mimeType);

				return stream;
			}

			extension = null;
			return null;
		}

		static string GetColumnValue(AndroidUri contentUri, string column, string selection = null, string[] selectionArgs = null)
		{
			try
			{
				var value = QueryContentResolverColumn(contentUri, column, selection, selectionArgs);
				if (!string.IsNullOrEmpty(value))
					return value;
			}
			catch
			{
				// Ignore all exceptions and use null for the error indicator
			}

			return null;
		}

		static string GetDataFilePath(AndroidUri contentUri, string selection = null, string[] selectionArgs = null)
		{
#pragma warning disable CS0618 // Type or member is obsolete
			const string column = MediaStore.Files.FileColumns.Data;
#pragma warning restore CS0618 // Type or member is obsolete

			// ask the content provider for the data column, which may contain the actual file path
			var path = GetColumnValue(contentUri, column, selection, selectionArgs);
			if (!string.IsNullOrEmpty(path) && Path.IsPathRooted(path))
				return path;

			return null;
		}

		static string GetFileExtension(AndroidUri uri)
		{
			var mimeType = Platform.ContentResolver.GetType(uri);

			return mimeType != null
				? MimeTypeMap.Singleton.GetExtensionFromMimeType(mimeType)
				: null;
		}

		static string QueryContentResolverColumn(AndroidUri contentUri, string columnName, string selection = null, string[] selectionArgs = null)
		{
			string text = null;

			var projection = new[] { columnName };
			using var cursor = Platform.ContentResolver.Query(contentUri, projection, selection, selectionArgs, null);
			if (cursor?.MoveToFirst() == true)
			{
				var columnIndex = cursor.GetColumnIndex(columnName);
				if (columnIndex != -1)
					text = cursor.GetString(columnIndex);
			}

			return text;
		}

	}
}

namespace Microsoft.Maui.Essentials
{
	public partial class FileBase
	{
		internal FileBase(Java.IO.File file)
			: this(file?.Path)
		{
		}

		string PlatformGetContentType(string extension) =>
			MimeTypeMap.Singleton.GetMimeTypeFromExtension(extension.TrimStart('.'));

		void PlatformInit(FileBase file)
		{
		}

		internal virtual Task<Stream> PlatformOpenReadAsync()
		{
			var stream = File.OpenRead(FullPath);
			return Task.FromResult<Stream>(stream);
		}
	}
}
