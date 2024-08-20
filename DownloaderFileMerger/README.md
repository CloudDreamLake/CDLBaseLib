# DownLoader Merge File Helper
this is a project to help cdl/MultiDownloader to save downloaded file.

Default merger will place each file into .tmp and merge when finished.

An example of acceleration is the use of memory mapping in your file merger.
## Api
### DownloaderFileMerger (namespace)
#### class FileMergerConfigure
##### string Save_path
##### int Index
#### class FileMerger
##### public void Main()
Thread's Entry Point.
##### public static FileMerger Create(FileMergerConfigure)
A static method for FileDownloader to create a merger.
##### public static void Intro()
A static method for Downloader to call to intro the merger.
##### public static bool Check()
A static method for Downloader to call to allow merger checking the environment.
If the merger can't run, return false.
else return true.
##### public void Stop ()
A method for FileDownloader to call to tell the merger that download finished.