using MultiDownloader;
using System.Net;

namespace MultiDownloader
{
    public interface IDownloadProgressListener
    {
        void OnDownloadSize(long size);
    }
    /// <summary>
    /// 下载进度更新显示
    /// </summary>
    public class DownloadProgressListener : IDownloadProgressListener
    {
        private long presize = 0;
        private DownMsg downMsg = null;

        public DownloadProgressListener(DownMsg downmsg)
        {
            this.downMsg = downmsg;
        }

        public delegate void DownSendMsg(DownMsg msg);

        public DownSendMsg doSendMsg = null;

        public void OnDownloadSize(long size)
        {
            if (downMsg == null)
            {
                DownMsg downMsg = new();
            }

            //下载速度
            if (downMsg.Size == 0)
            {
                downMsg.Speed = size;
            }
            else
            {
                downMsg.Speed = (float)(size - downMsg.Size);
            }
            if (downMsg.Speed == 0)
            {
                downMsg.Surplus = -1;
                downMsg.SurplusInfo = "未知";
            }
            else
            {
                downMsg.Surplus = ((downMsg.Length - downMsg.Size) / downMsg.Speed);
            }
            downMsg.Size = size; //下载总量

            if (size == downMsg.Length)
            {
                //下载完成
                downMsg.Tag = DownStatus.End;
                downMsg.SpeedInfo = "0 K";
                downMsg.SurplusInfo = "已完成";
            }
            else
            {
                //下载中
                downMsg.Tag = DownStatus.DownLoad;
            }

            if (doSendMsg != null) doSendMsg(downMsg);//通知具体调用者下载进度
        }
    }

    /// <summary>
    /// 下载类型
    /// </summary>
    public enum DownStatus
    {
        Start,
        GetLength,
        DownLoad,
        End,
        Error
    }

    public class DownMsg
    {
        private int _Length = 0;
        private string _LengthInfo = "";
        private int _Id = 0;
        private DownStatus _Tag = 0;
        private long _Size = 0;
        private string _SizeInfo = "";
        private float _Speed = 0;
        private float _Surplus = 0;
        private string _SurplusInfo = "";
        private string _ErrMessage = "";
        private string _SpeedInfo = "";
        private double _Progress = 0;

        public int Length
        {
            get
            {
                return _Length;
            }

            set
            {
                _Length = value;
                LengthInfo = GetFileSize(value);
            }
        }

        public int Id
        {
            get
            {
                return _Id;
            }

            set
            {
                _Id = value;
            }
        }

        /// </summary>
        public DownStatus Tag
        {
            get
            {
                return _Tag;
            }

            set
            {
                _Tag = value;
            }
        }

        public long Size
        {
            get
            {
                return _Size;
            }

            set
            {
                _Size = value;
                SizeInfo = GetFileSize(value);
                if (Length >= value)
                {
                    Progress = Math.Round((double)value / Length * 100, 2);
                }
                else
                {
                    Progress = -1;
                }
            }
        }

        public float Speed
        {
            get
            {
                return _Speed;
            }

            set
            {
                _Speed = value;
                SpeedInfo = GetFileSize(value);
            }
        }

        public string SpeedInfo
        {
            get
            {
                return _SpeedInfo;
            }

            set
            {
                _SpeedInfo = value;
            }
        }

        public float Surplus
        {
            get
            {
                return _Surplus;
            }

            set
            {
                _Surplus = value;
                if (value > 0)
                {
                    SurplusInfo = GetDateName((int)Math.Round(value, 0));
                }
            }
        }

        public string ErrMessage
        {
            get
            {
                return _ErrMessage;
            }

            set
            {
                _ErrMessage = value;
            }
        }

        public string SizeInfo
        {
            get
            {
                return _SizeInfo;
            }

            set
            {
                _SizeInfo = value;
            }
        }

        public string LengthInfo
        {
            get
            {
                return _LengthInfo;
            }

            set
            {
                _LengthInfo = value;
            }
        }

        public double Progress
        {
            get
            {
                return _Progress;
            }

            set
            {
                _Progress = value;
            }
        }

        public string SurplusInfo
        {
            get
            {
                return _SurplusInfo;
            }

            set
            {
                _SurplusInfo = value;
            }
        }

        private string GetFileSize(float Len)
        {
            float temp = Len;
            string[] sizes = { "B", "KB", "MB", "GB" };
            int order = 0;
            while (temp >= 1024 && order + 1 < sizes.Length)
            {
                order++;
                temp = temp / 1024;
            }
            return String.Format("{0:0.##} {1}", temp, sizes[order]);
        }

        private string GetDateName(int Second)
        {
            float temp = Second;
            string suf = "秒";
            if (temp > 60)
            {
                suf = "分钟";
                temp /= 60;
                if (temp > 60)
                {
                    suf = "小时";
                    temp /= 60;
                    if (temp > 24)
                    {
                        suf = "天";
                        temp /= 24;
                        if (temp > 30)
                        {
                            suf = "月";
                            temp /= 30;
                            if (temp > 12)
                            {
                                suf = "年";
                                temp /= 12;
                            }
                        }
                    }
                }
            }

            return String.Format("{0:0} {1}", temp, suf);
        }
    }
}
/// <summary>
/// 文件下载
/// </summary>
public class FileDownloader
{
    /// <summary>
    /// 已下载文件长度
    /// </summary>
    private long downloadSize = 0;

    /// <summary>
    /// 原始文件长度
    /// </summary>
    private long fileSize = 0;

    /// <summary>
    /// 线程数
    /// </summary>
    private DownloadThread[] threads;

    /// <summary>
    /// 本地保存文件
    /// </summary>
    private string saveFile;

    /// <summary>
    /// 缓存各线程下载的长度
    /// </summary>
    public Dictionary<int, long> data = new Dictionary<int, long>();

    /// <summary>
    /// 每条线程下载的长度
    /// </summary>
    private long block;

    /// <summary>
    /// 下载路径
    /// </summary>
    private String downloadUrl;

    /// <summary>
    ///  获取线程数
    /// </summary>
    /// <returns> 获取线程数</returns>
    public int getThreadSize()
    {
        return threads.Length;
    }

    /// <summary>
    ///   获取文件大小
    /// </summary>
    /// <returns>获取文件大小</returns>
    public long getFileSize()
    {
        return fileSize;
    }

    /// <summary>
    /// 累计已下载大小
    /// </summary>
    /// <param name="size">累计已下载大小</param>
    public void append(long size)
    {
        //锁定同步..............线程开多了竟然没有同步起来.文件下载已经完毕了,下载总数量却不等于文件实际大小,找了半天原来这里错误的
        lock (this)
        {
            downloadSize += size;
        }
    }

    /// <summary>
    /// 更新指定线程最后下载的位置
    /// </summary>
    /// <param name="threadId">threadId 线程id</param>
    /// <param name="pos">最后下载的位置</param>
    public void update(int threadId, long pos)
    {
        if (data.ContainsKey(threadId))
        {
            this.data[threadId] = pos;
        }
        else
        {
            this.data.Add(threadId, pos);
        }
    }

    /// <summary>
    /// 构建下载准备,获取文件大小
    /// </summary>
    /// <param name="downloadUrl">下载路径</param>
    /// <param name="fileSaveDir"> 文件保存目录</param>
    /// <param name="threadNum">下载线程数</param>
    public FileDownloader(string downloadUrl, string fileSaveDir, string filename = "", int threadNum = 3)
    {
        try
        {
            if (string.IsNullOrEmpty(filename))
            {
                filename = Uri.UnescapeDataString(Path.GetFileName(downloadUrl));//获取文件名称 uri 解码中文字符
            }
            //构建http 请求
            this.downloadUrl = downloadUrl;
            if (!Directory.Exists(fileSaveDir)) Directory.CreateDirectory(fileSaveDir);
            this.threads = new DownloadThread[threadNum];
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(downloadUrl);
            request.Referer = downloadUrl.ToString();
            request.Method = "GET";
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; SV1; .NET CLR 2.0.1124)";
            request.ContentType = "application/octet-stream";
            request.Accept = "image/gif, image/jpeg, image/pjpeg, image/pjpeg, application/x-shockwave-flash, application/xaml+xml, application/vnd.ms-xpsdocument, application/x-ms-xbap, application/x-ms-application, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*";
            request.Timeout = 20 * 1000;
            request.AllowAutoRedirect = true;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    this.fileSize = response.ContentLength;//根据响应获取文件大小
                    if (this.fileSize <= 0) throw new Exception("获取文件大小失败");

                    if (filename.Length == 0) throw new Exception("获取文件名失败");
                    this.saveFile = Path.Combine(fileSaveDir, filename); //构建保存文件
                                                                         //计算每条线程下载的数据长度
                    this.block = (this.fileSize % this.threads.Length) == 0 ? this.fileSize / this.threads.Length : this.fileSize / this.threads.Length + 1;
                }
                else
                {
                    throw new Exception("服务器返回状态失败,StatusCode:" + response.StatusCode);
                }
            }
        }
        catch (Exception e)
        {
            throw new Exception("无法连接下载地址");
        }
    }

    /// <summary>
    /// 开始下载文件
    /// </summary>
    /// <param name="listener">监听下载数量的变化,如果不需要了解实时下载的数量,可以设置为null</param>
    /// <returns>已下载文件大小</returns>
    public long download(IDownloadProgressListener listener)
    {
        try
        {
            using (FileStream fstream = new FileStream(this.saveFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                if (this.fileSize > 0) fstream.SetLength(this.fileSize);
                fstream.Close();
            }
            if (this.data.Count != this.threads.Length)
            {
                this.data.Clear();
                for (int i = 0; i < this.threads.Length; i++)
                {
                    this.data.Add(i + 1, 0);//初始化每条线程已经下载的数据长度为0
                }
            }
            for (int i = 0; i < this.threads.Length; i++)
            {
                //开启线程进行下载
                long downLength = this.data[i + 1];
                if (downLength < this.block && this.downloadSize < this.fileSize)
                {//判断线程是否已经完成下载,否则继续下载	+
                    this.threads[i] = new DownloadThread(this, downloadUrl, this.saveFile, this.block, this.data[i + 1], i + 1);
                    this.threads[i].ThreadRun();
                }
                else
                {
                    this.threads[i] = null;
                }
            }
            bool notFinish = true;//下载未完成
            while (notFinish)
            {
                // 循环判断所有线程是否完成下载
                Thread.Sleep(900);
                notFinish = false;//假定全部线程下载完成
                for (int i = 0; i < this.threads.Length; i++)
                {
                    if (this.threads[i] != null && !this.threads[i].isFinish())
                    {
                        //如果发现线程未完成下载
                        notFinish = true;//设置标志为下载没有完成
                        if (this.threads[i].getDownLength() == -1)
                        {
                            //如果下载失败,再重新下载
                            this.threads[i] = new DownloadThread(this, downloadUrl, this.saveFile, this.block, this.data[i + 1], i + 1);
                            this.threads[i].ThreadRun();
                        }
                    }
                }
                if (listener != null)
                {
                    listener.OnDownloadSize(this.downloadSize);//通知目前已经下载完成的数据长度
                    Console.WriteLine(this.downloadSize);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw new Exception("下载文件失败");
        }
        return this.downloadSize;
    }
}
/// <summary>
/// 线程下载
/// </summary>
public class DownloadThread
{
    private string saveFilePath;
    private string downUrl;
    private long block;
    private int threadId = -1;
    private long downLength;
    private bool finish = false;
    private FileDownloader downloader;

    public DownloadThread(FileDownloader downloader, string downUrl, string saveFile, long block, long downLength, int threadId)
    {
        this.downUrl = downUrl;
        this.saveFilePath = saveFile;
        this.block = block;
        this.downloader = downloader;
        this.threadId = threadId;
        this.downLength = downLength;
    }

    public void ThreadRun()
    {
        //task
        Thread td = new Thread(new ThreadStart(() =>
        {
            if (downLength < block)//未下载完成
            {
                try
                {
                    int startPos = (int)(block * (threadId - 1) + downLength);//开始位置
                    int endPos = (int)(block * threadId - 1);//结束位置
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(downUrl);
                    request.Referer = downUrl.ToString();
                    request.Method = "GET";
                    request.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; SV1; .NET CLR 2.0.1124)";
                    request.AllowAutoRedirect = false;
                    request.ContentType = "application/octet-stream";
                    request.Accept = "image/gif, image/jpeg, image/pjpeg, image/pjpeg, application/x-shockwave-flash, application/xaml+xml, application/vnd.ms-xpsdocument, application/x-ms-xbap, application/x-ms-application, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*";
                    request.Timeout = 10 * 1000;
                    request.AllowAutoRedirect = true;
                    request.AddRange(startPos, endPos);
                    //Console.WriteLine(request.Headers.ToString()); //输出构建的http 表头
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    WebResponse wb = request.GetResponse();
                    using (Stream _stream = wb.GetResponseStream())
                    {
                        byte[] buffer = new byte[1024 * 50]; //缓冲区大小
                        long offset = -1;
                        using (Stream threadfile = new FileStream(this.saveFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite)) //设置文件以共享方式读写,否则会出现当前文件被另一个文件使用.
                        {
                            threadfile.Seek(startPos, SeekOrigin.Begin); //移动文件位置
                            while ((offset = _stream.Read(buffer, 0, buffer.Length)) != 0)
                            {
                                //offset 实际下载流大小
                                downloader.append(offset); //更新已经下载当前总文件大小
                                threadfile.Write(buffer, 0, (int)offset);
                                downLength += offset;  //设置当前线程已下载位置
                                downloader.update(this.threadId, downLength);
                            }
                            this.finish = true;
                        }
                    }
                }
                catch (Exception e)
                {
                    this.downLength = -1;
                }
            }
        }));
        td.IsBackground = true;
        td.Start();
    }

    /// <summary>
    /// 下载是否完成
    /// </summary>
    /// <returns></returns>
    public bool isFinish()
    {
        return finish;
    }

    /// <summary>
    ///  已经下载的内容大小
    /// </summary>
    /// <returns>如果返回值为-1,代表下载失败</returns>
    public long getDownLength()
    {
        return downLength;
    }
}
public class MultiDownload
{
    public int ThreadNum = 3;
    private List<Thread> list = new List<Thread>();

    public MultiDownload()
    {
        doSendMsg += Change;
    }

    private void Change(DownMsg msg)
    {
        if (msg.Tag == DownStatus.Error || msg.Tag == DownStatus.End)
        {
            StartDown(1);
        }
    }

    /// <summary>
    /// 添加下载
    /// </summary>
    /// <param name="DownUrl">下载路径</param>
    /// <param name="Dir">保存路径</param>
    /// <param name="Id">id</param>
    /// <param name="FileName">保存的文件名</param>
    public void AddDown(string downUrl, string dir, int id = 0, string fileName = "")
    {
        Thread tsk = new Thread(() =>
        {
            download(downUrl, dir, fileName, id);
        });
        list.Add(tsk);
    }

    /// <summary>
    /// 准备开始下载
    /// </summary>
    /// <param name="StartNum">启动下载的任务数</param>
    public void StartDown(int startNum = 3)
    {
        for (int i2 = 0; i2 < startNum; i2++)
        {
            lock (list)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].ThreadState == System.Threading.ThreadState.Unstarted || list[i].ThreadState == ThreadState.Suspended)
                    {
                        list[i].Start();
                        break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 委托回调
    /// </summary>
    /// <param name="msg"></param>
    public delegate void DownloadSendMsg(DownMsg msg);

    /// <summary>
    /// 委托回调
    /// </summary>
    public event DownloadSendMsg doSendMsg;

    /// <summary>
    /// 下载
    /// </summary>
    /// <param name="path">下载路径</param>
    /// <param name="dir">保存文件夹</param>
    /// <param name="filename">保存文件名</param>
    /// <param name="id"></param>
    private void download(string path, string dir, string filename, int id = 0)
    {
        try
        {
            DownMsg msg = new DownMsg();
            msg.Id = id;
            msg.Tag = 0;
            doSendMsg(msg);
            FileDownloader loader = new FileDownloader(path, dir, filename, ThreadNum);
            loader.data.Clear();
            msg.Tag = DownStatus.Start;
            msg.Length = (int)loader.getFileSize();
            doSendMsg(msg);
            DownloadProgressListener linstenter = new DownloadProgressListener(msg);
            linstenter.doSendMsg = new DownloadProgressListener.DownSendMsg(doSendMsg);
            loader.download(linstenter);
        }
        catch (Exception ex)
        {
            DownMsg msg = new DownMsg();
            msg.Id = id;
            msg.Length = 0;
            msg.Tag = DownStatus.Error;
            msg.ErrMessage = ex.Message;
            doSendMsg(msg);
        }
    }
}