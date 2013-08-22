using System;
using System.Threading;
using LiveDebugger;


namespace WarZ
{
#if WINDOWS || XBOX
    static class Program
    {
        private static LiveDebuggerWindow LDWindow;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            /*
           www
            

            Thread liveDebugger = new Thread(StartLiveDebugger);
            liveDebugger.IsBackground = true;
            liveDebugger.SetApartmentState(ApartmentState.STA);
            liveDebugger.Start();


            
            
            while (!LDbg.Initialized) ;
            */
            
            using (WarZGame game = new WarZGame())
            {
                game.Run();
            }
        }

        private static void StartLiveDebugger()
        {
            LDWindow = new LiveDebuggerWindow();
            //LDWindow.listBox1.ItemsSource = "wazaaa";
            LDWindow.Show();
            LDbg.Initialize(LDWindow);
            //LDbg.Initialize(LDWindow);
            LDbg.AddStack("wazaaaa");
            for (; ; ) ;
                
        }
    }
#endif
}

