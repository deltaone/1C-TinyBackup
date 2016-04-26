using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;

using ZLibNet;

///Similar .NET collections (candidates to use instead of Dictionary and Hashtable):
///    ConcurrentDictionary - thread safe (can be safely accessed from several threads concurrently)
///    HybridDictionary - optimized performance (for few items and also for many items)
///    OrderedDictionary - values can be accessed via int index (by order in which items were added)
///    SortedDictionary - items automatically sorted
///    StringDictionary - strongly typed and optimized for strings

namespace Core
{
    static class Task
    {
        public static string mode = GM.ReadProfileString("main", "mode", "0");
        public static string backupFolder = ".\\";
        public static Dictionary<string, Dictionary<string, string>> records = new Dictionary<string, Dictionary<string, string>>();

        public static bool isPaused = false;
        public static bool isExiting = false;

        private static Zipper zip = new Zipper();

        static Task()
        {
            GM.LogScopeSet("READ-CONFIG");

            var sections = new List<string>() { "primary", "secondary" };
            foreach (var section in sections)
            {
                var folder = GM.ReadProfileString("main", section, "");
                if (!folder.IsEmpty() && Directory.Exists(folder))
                {
                    backupFolder = folder;
                    break;
                }
            }
            
            GM.Log("mode = '" + mode + "'");
            GM.Print("backup folder = '" + backupFolder + "'");

            var keys = new Dictionary<string, string>()
            {
                {"enabled", "1"},
                {"folder", ""},
                {"execute", ""},
                {"skip", ""},
                {"mask", "*"},
                {"filter", ""},
                {"exclude", ""},
                {"recursive", "1"},
                {"relative", "1"},
                {"depth", "7"},
                {"files", "0"},
                {"size", "0"},
                {"time", "0"}
            };

            int i = 1;
            while (true)
            {
                string section = i.ToString();
                string id = GM.ReadProfileString(section, "id");
                if (id.IsEmpty()) break;
                records[id] = new Dictionary<string, string>();
                records[id]["section"] = section;
                records[id]["id"] = id;
                records[id]["title"] = GM.ReadProfileString(section, "title", id);
                foreach (var e in keys) records[id][e.Key] = GM.ReadProfileString(section, e.Key, e.Value);
                if (records[id]["folder"].IsEmpty() || !Directory.Exists(records[id]["folder"]))
                {
                    GM.Log("Task [" + i.ToString() + "] skipped, source folder '" + records[id]["folder"] + "' not found!");
                    records.Remove(id);
                    i++;
                    continue;
                }
                GM.Log("Task added - [" + i.ToString() + "][" + records[id]["id"] + "][" + records[id]["title"] + "]:");
                foreach (var e in keys) GM.Log("   '" + e.Key + "' = '" + records[id][e.Key] + "'");
                i++;
            }
            
            GM.LogScopeRestore();
        }

        private static bool IsTaskCanStart(Dictionary<string, string> task, out string reason)
        {
            if(task["enabled"] == "0") 
            {
                reason = "task not enabled";
                return (false);
            }
            if(!task["execute"].IsEmpty() && File.Exists(Path.Combine(task["folder"], task["execute"]))) 
            {
                reason = "file-flag found";
                return (true);
            }
            if(!task["skip"].IsEmpty() && File.Exists(Path.Combine(task["folder"], task["skip"])))
            {
                reason = "file-flag found";
                return (false);
            }
            
            Int64 size = 0;
            DateTime time = DateTime.MinValue;
            var files = Directory.GetFiles(task["folder"], "*", SearchOption.AllDirectories);
            foreach (var f in files)
            {
                FileInfo fi = new FileInfo(f);
                size += fi.Length;
                if (time < fi.LastWriteTime) time = fi.LastWriteTime;
            }

            var strFiles = files.Length.ToString();
            var strTime = time.ToString();
            var strSize = size.ToString();
            if (strFiles != task["files"] || strTime != task["time"] || strSize != task["size"])
            {
                if (strFiles != task["files"]) reason = "folder file number changed";
                else if (strTime != task["time"]) reason = "folder time changed";
                else reason = "folder size changed";
                task["files"] = strFiles;
                task["time"] = strTime;
                task["size"] = strSize;
                return (true);
            }            

            reason = "folder unchanged";
            return (false);
        }

        private static void RemoveOutdatedBackups(string id, int depth)
        {
            var files = new List<string>(Directory.GetFiles(backupFolder, "-" + id + "-*.zip", SearchOption.TopDirectoryOnly));
            // files.Sort((a, b) => a.CompareTo(b)); // ascending sort
            // files.Sort((a, b) => -1 * a.CompareTo(b)); // descending sort
            if (files.Count <= depth) return;

            files.Sort();
            files.Reverse();
            files.RemoveRange(0, depth);

            foreach (var f in files)
            {
                GM.Print("Remove outdated backup: " + f);
                File.Delete(f);
            }
        }

        public static void DoTask()
        {
            GM.Messenger.Broadcast("TASK", true);            
            GM.LogScopeSet("TASK");

            GM.Print("Backup folder: '" + backupFolder + "'");

            foreach (var r in records)
            {
                if (isExiting) break;
                while (isPaused) Thread.Sleep(250);

                string reason;
                string message;
                var task = r.Value;

                if (IsTaskCanStart(task, out reason))
                {
                    message = "Starting task '" + task["id"] + "' [" + task["title"] + "] (" + reason + ") ...";
                    GM.Messenger.Broadcast("BALLOON", new List<object> { TimeSpan.FromSeconds(3), "Note!", message, ToolTipIcon.Info});
                    GM.Print(message);
                }
                else
                {
                    message = "Skipping task '" + task["id"] + "' [" + task["title"] + "] (" + reason + ") ...";
                    GM.Messenger.Broadcast("BALLOON", new List<object> { TimeSpan.FromSeconds(3), "Note!", message, ToolTipIcon.Info });
                    GM.Print(message);
                    continue;
                }

                GM.WriteProfileString(task["section"], "files", task["files"]);
                GM.WriteProfileString(task["section"], "time", task["time"]);
                GM.WriteProfileString(task["section"], "size", task["size"]);

                GM.LogScopeSet("ZIP");

                zip = new Zipper();
                zip.Recurse = (task["recursive"] == "1" ? true : false);
                zip.PathInZip = (task["relative"] == "1" ? enPathInZip.Relative : enPathInZip.Absolute);
                zip.ZipFile = Path.Combine(backupFolder, String.Format("-{0}-{1}.zip", task["id"], DateTime.Now.ToString("yyyyMMdd-HHmmss")));
                GM.Print("Source: '" + task["folder"] + "'");
                GM.Print("Destination: '" + zip.ZipFile + "'");

                var a = task["mask"].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string e in a) zip.ItemList.Add(Path.Combine(task["folder"], e));

                a = task["exclude"].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string e in a) zip.ExcludeFollowing.Add(e);

                a = task["filter"].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string e in a) zip.IncludeOnlyFollowing.Add(e);

                zip.eventExeptionWarning += GM.Warning;
                zip.eventFileAdding += onZipAddingFile;
                zip.eventFileSkipping += onZipSkippingFile;
                zip.eventFilePercent += onZipFilePercent;

                try
                {
                    zip.Zip();
                }
                catch (Exception ex)
                {
                    GM.Warning(ex.Message);
                }

                RemoveOutdatedBackups(task["id"], task["depth"].Parse(7));

                GM.LogScopeRestore();                
            }

            Wav.Play(Path.Combine(GM.folderExecutable, "beep.wav"));

            GM.Messenger.Broadcast("PROGRESS", 0);
            GM.Messenger.Broadcast("STATUS", "Finished!");
            GM.Messenger.Broadcast("TASK", false);
            GM.Print("Done!");

            GM.Messenger.Broadcast("BALLOON", new List<object> { TimeSpan.FromSeconds(3), "Note!", "All done!", ToolTipIcon.Info });

            GM.LogScopeRestore();

            if (Task.mode == "0") GM.Messenger.Broadcast("EXIT");
        }

        public static void onZipAddingFile(string file, int number, int total)
        {            
            string message = "[" + (number + 1) + "/" + total + "] Adding: '" + file + "'";
            GM.Messenger.Broadcast("STATUS", message);
            GM.Log(message);
        }

        public static void onZipSkippingFile(string file, int number, int total)
        {
            string message = "[" + (number + 1) + "/" + total + "] Skipping: '" + file + "'";
            GM.Messenger.Broadcast("STATUS", message);
            GM.Log(message);
        }

        public static void onZipFilePercent(int percent)
        {
            GM.Messenger.Broadcast("PROGRESS", percent);
        }

        public static void onPause(ref object arg)
        {
            isPaused = !isPaused;
            if (isPaused) GM.Print("Paused !");
            else GM.Print("Unpaused !");
            zip.Pause(isPaused);
        }

        public static void onExit(ref object arg)
        {
            isExiting = true;
            zip.Interrupt();
        }
    }
}
