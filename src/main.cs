// #define CONSOLE
// don't forget setup console application into project settings
#define SINGLE_INSTANCE

using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Reflection;
using System.Text;

#if !CONSOLE
using System.Windows.Forms;
#endif

[assembly: AssemblyTitle(Core.GM.assemblyTitle)]
[assembly: AssemblyDescription(Core.GM.assemblyDescription)]
[assembly: AssemblyCopyright(Core.GM.assemblyCopyright)]
[assembly: AssemblyConfiguration(Core.GM.assemblyConfiguration)]
[assembly: AssemblyCompany(Core.GM.assemblyCompany)]
[assembly: AssemblyProduct(Core.GM.assemblyProduct)]
[assembly: AssemblyTrademark(Core.GM.assemblyTrademark)]
[assembly: AssemblyCulture(Core.GM.assemblyCulture)]
[assembly: ComVisible(Core.GM.assemblyCOMVisible)]
[assembly: Guid(Core.GM.assemblyGUID)]
[assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyFileVersion("1.0.0.0")]

namespace Core {
public class GM
{   // program values
    public static bool isLogEnabled = true;
    public static bool isPauseAfterExit = true;
    public const string assemblyTitle = "Tiny Backup";
    public const string assemblyDescription = "Backup files and folders ...";
    public const string assemblyCopyright = "@ 2016 / de1ta0ne";
    public const string helpCommandLine = "[KEYS]";
    public const string helpKeys = "\nKeys:\n-h  help\n-d  debug mode on\n-v  verbose mode on\n";
    public const string assemblyGUID = "71865971-61B8-4A3E-9274-4C59E0D5EB5E";    
    public const string assemblyConfiguration = "retail";
    public const string assemblyCompany = "";
    public const string assemblyTrademark = "";
    public const string assemblyCulture = "";
    public const string assemblyProduct = assemblyTitle;
    public const bool   assemblyCOMVisible = false;
    private static Version _assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
    public static readonly string assemblyVersion = _assemblyVersion.Major.ToString() + "." + _assemblyVersion.Minor.ToString() + " build " + _assemblyVersion.Build;
    public static readonly string assemblyDate = (new DateTime(2000, 1, 1).AddDays(_assemblyVersion.Build).AddSeconds(_assemblyVersion.Revision * 2)).ToString("dd.MM.yyyy");
    public static readonly string version = "v" + assemblyVersion + " [" + assemblyDate + "]";
    public static bool isVerbose;
    public static bool isDebug;
    // patches
    public static readonly string fileExecutable = Assembly.GetExecutingAssembly().Location;
    public static readonly string folderExecutable = Path.GetDirectoryName(fileExecutable) + @"\";
    public static readonly string folderCurrent = Directory.GetCurrentDirectory() + @"\";
    public static readonly string fileIni = Path.Combine(folderExecutable, Path.GetFileNameWithoutExtension(fileExecutable) + ".ini");
    public static readonly string fileLog = Path.Combine(folderExecutable, Path.GetFileNameWithoutExtension(fileExecutable) + ".log");
    // subclasses
    public static MessageManager Messenger = new MessageManager();
    // output
    public static event Action<string> PrintEventHandler;
    private static Stack<string> _logScopeStack = new Stack<string>();
    
    // ------------------------------------------------------------------------
    
    static GM()
    {
        AppDomain.CurrentDomain.ProcessExit += ClassDestructor;

        #if DEBUG
            isDebug = true;
            isLogEnabled = true;
        #else                
            isDebug = false;
        #endif

        #if CONSOLE
            isVerbose = false;
            Console.SetWindowSize(120, 45);
            GM.PrintEventHandler += Console.WriteLine;
        #else
            isVerbose = true;
        #endif

        File.Delete(fileLog);
        Logger.SetLogPath(fileLog);
        GM.Log("Log started: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff"));
    }

    private static void ClassDestructor(object sender, EventArgs e)
    {

    }

    // ------------------------------------------------------------------------
    
    public static bool Startup(List<string> args, params object[] list)
    {
        //if (args.Count < 1 || !File.Exists(args[0])) return (false);
        return (true);
    }

    public static void DoTask()
    {
        #if SINGLE_INSTANCE
        bool result;
        var mutex = new System.Threading.Mutex(true, "de1ta0ne-backup", out result);
        if (!result)
        {
            MessageBox.Show("Another instance is already running.");
            return;
        }
        #endif
        
        #if !CONSOLE
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new Mainframe());
        #else
        #endif

        #if SINGLE_INSTANCE
        GC.KeepAlive(mutex);  // mutex shouldn't be released - important line
        #endif
    }

    public static void Shutdown()
    {
        
    }

    // ------------------------------------------------------------------------
    
    [STAThread]
    private static void Main(string[] args)
    {
        string message = assemblyTitle + " " + version + "\n";
        Print(message);

        bool help = false;
        var extra = new List<string>();
        var p = new Mono.Options.OptionSet() {        
        { "d|debug",    v => {isDebug = true;}},
        { "v|verbose",  v => {isVerbose = true;}},
        { "h|?|help",   v => {help = true;}}, // { "l|log=",     v => GM.fileLog = v },
        };
        try { extra = p.Parse(args); }
        catch (Exception ex) { Warning(ex.Message); }; // GM.Error("\nEXCEPTION:\n" + ex.Message + "\n" + ex.StackTrace);        

        if (help || (args.Length == 0 && !isVerbose))
        {
            string usage = "Usage:\n   " + Path.GetFileName(fileExecutable) + " " + helpCommandLine + "\n" + helpKeys + "\n" + assemblyCopyright + "\n";
            Print(usage);
            #if !CONSOLE
            MessageBox.Show(message + usage, "Note ...");
            #endif
        }
        else if (Startup(extra))
        {
            DoTask();
            Shutdown();
            Print("\nDone!");
        } 
        else 
        {
            Log("\nStartup failed, exiting ...");
        }
        
        #if CONSOLE
        if (isPauseAfterExit) Console.ReadKey(true);
        #endif
    }

    //---------------------------------------------------------------------

    public static void Print(string message)
    {
        try
        {
            if (PrintEventHandler != null) PrintEventHandler(message);
            Log(message);
        }
        catch (Exception ex)
        {
            Log("[WARNING] Print() - exception on EventHandlerOnPrint()\n" + ex.Message + "\n" + ex.StackTrace);
        }
    }

    public static void Warning(string message)
    {
        Print("[WARNING] " + message);
    }

    public static void Warning(string message, bool caller = false, bool trace = false)
    {        
        var i = GetCallerInfo(caller ? 3 : 2);
        Print("[WARNING] " + message + String.Format(" at <{0}/{1}()[{2}]>", i["file"], i["method"], i["line"]));
        if (trace) foreach (var e in GetStackTrace(2)) Log("[WARNING] " + e);
    }

    public static void Error(string message)
    {
        var i = GetCallerInfo(2);
        Print("[ERROR] " + message + String.Format(" at <{0}/{1}()[{2}]>", i["file"], i["method"], i["line"]));
        foreach (var e in GetStackTrace(2)) Log("[ERROR] " + e);
    }

    public static void Critical(string message)
    {
        var i = GetCallerInfo(2);
        Print("[CRITICAL] " + message + String.Format(" at <{0}/{1}()[{2}]>", i["file"], i["method"], i["line"]));
        foreach (var e in GetStackTrace(2)) Log("[CRITICAL] " + e);
        MessageBox.Show("[CRITICAL] " + message);
        Application.Exit();
    }

    public static void Debug(string message)
    {
        if (!isDebug) return;
        var i = GetCallerInfo(2);
        Print(String.Format("[DEBUG] {0}(): {1}", i["method"], message));
    }

    //---------------------------------------------------------------------

    public static Dictionary<string, string> GetCallerInfo(int frame = 1)
    {   // MethodBase.GetCurrentMethod().Name
        var f = new StackFrame(frame, true);
        return (new Dictionary<string, string> { { "method", f.GetMethod().Name }, 
                    { "file", Path.GetFileName(f.GetFileName()) },  { "line", f.GetFileLineNumber().ToString() }});
    }

    public static List<string> GetStackTrace(int skip = 2)
    {
        var result = new List<string>();
        var stack = new System.Diagnostics.StackTrace(true).ToString();
        var trace = new List<string>(stack.Split('\n'));

        trace.RemoveRange(0, skip);
        trace.Reverse();

        bool found = false;
        foreach (string e in trace)
        {
            if (!found && e.Contains("Main")) found = true;
            if (found) result.Add(e.Replace('\n', ' ').Replace("\r", "").Replace('\t', ' '));
        }
        result.Reverse();
        return (result);
    }

    //---------------------------------------------------------------------
    public static void LogScopeSet(string scope) 
    {
        _logScopeStack.Push(scope);
    }

    public static void LogScopeRestore()
    {
        _logScopeStack.Pop();        
    }

    public static void Log(string scope, string message, bool flush = false)
    {
        if (isLogEnabled) Logger.Write("[" + scope + "] " + message, flush);
    }

    public static void Log(string message)
    {
        if (!isLogEnabled) return;

        if (_logScopeStack.Count > 0) Logger.Write("[" + _logScopeStack.Peek() + "] " + message, false);
        else Logger.Write(message, false);
    }

    // ------------------------------------------------------------------------    

	[DllImport("kernel32")]
	private static extern long WritePrivateProfileString(string section, string key, string value, string filePath);

	[DllImport("kernel32")]
	private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder value, int size, string filePath);

    public static void WriteProfileString(string section, string key, string value)
	{
		WritePrivateProfileString(section, key, value, fileIni);
	}

    public static string ReadProfileString(string section, string key, string def = "")
	{
		StringBuilder temp = new StringBuilder(1024);
        int i = GetPrivateProfileString(section, key, def, temp, 1024, fileIni);
		return temp.ToString();
	}

    //---------------------------------------------------------------------

    public static void DelayedCall(Action method, int delay)
    {
        //System.Threading.Timer timer = null;
        //timer = new System.Threading.Timer((obj) =>
        //{
        //    doExit();
        //    timer.Dispose();
        //}, null, 1000, System.Threading.Timeout.Infinite);

        System.Threading.Timer timer = null;
        var cb = new System.Threading.TimerCallback((state) => { method(); timer.Dispose(); });
        timer = new System.Threading.Timer(cb, null, delay, System.Threading.Timeout.Infinite);
    }
    
    public static string ResourceGetTextFile(string file)
    {
        // вариант 1 
        // добавь в проект файл как ресурс (Properties -> Resources -> AddFiles), и дальше в проекте 
        // обращайся Properties.Resources.<имя_ресурса>

        // вариант 2
        // добавь текстовый файл в проект
        // измени его свойство BuildAction на EmbededResource (правый клик на добавленном файле - properties)
        // идентификатор для загрузки ресурса - "text2table.page_begin.txt"
        StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(file));
        return (reader.ReadToEnd());
    }

    // ------------------------------------------------------------------------
}
}