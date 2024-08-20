using CDLLogger;

namespace MultiDownloader
{
    internal class FileDownloader
    {
        private readonly Logger logger;
        private readonly Downloader.DownloadInfo info;
        private readonly FileStream? filestream;
        private readonly StreamWriter? filewriter;
        public FileDownloader(Logger logger, Downloader.DownloadInfo info)
        {
            this.logger = logger;
            this.info = info;
            try
            {
                filestream = File.Open(info.file_name, FileMode.OpenOrCreate);

                filewriter = new StreamWriter(filestream);

                logger.Info("Download " + info.url.ToString() + " to file <" + info.file_name + ">");
            }
            catch (UnauthorizedAccessException ex)
            {
                logger.Error("File <" + info.file_name + "> is not available\n" + ex.ToString());
                return;
            }
            catch (IOException ex)
            {
                logger.Error("File <" + info.file_name + "> is not available\n" + ex.ToString());
                return;
            }
        }

        public void Download()
        {
            if (filestream == null || filewriter == null) return; 
            logger.Info("Start Download " + info.url.ToString());
        }
    }
}
