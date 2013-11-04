/*
 * author: xiaofeng.li
 * mail: 453588006
 * desc: REPL for c#
 * */

using System;
using System.CodeDom.Compiler;
using System.Reflection;
using Microsoft.CSharp;
using System.Threading;

namespace ns_behaviour
{
    public class CSRepl
    {
        string mScript = null;
        public static string mResult = null;

        static bool mExit = false;
        public static void exit() { mExit = true; }

        enum EState { e_normal, e_warning, e_error }
        EState mState = EState.e_normal;

        Thread mThread = null;

        String mCodeFrame = @"
using System;
using System.Collections.Generic;

public static class Wrapper
{{
    public static void print(Object o)
    {{
        Console.WriteLine(o.ToString() );
        //ns_GameViewer.CSRepl.mResult = o.ToString();
    }}
    public static void exit()
    {{
        ns_behaviour.CSRepl.stop();
    }}
    public static void PerformAction()
    {{  
        {0};// user code goes here
    }}
}}";

        CSharpCodeProvider mCodeProvide = null;
        CompilerParameters mCompileOptions = null;

        void init()
        {
            mCodeProvide = new CSharpCodeProvider();
            mCompileOptions = new CompilerParameters();

            mCompileOptions.GenerateInMemory = true;
            mCompileOptions.GenerateExecutable = false;

            // bring in system libraries
            mCompileOptions.ReferencedAssemblies.Add(typeof(CSRepl).Assembly.Location);
        }

        public void print(string info)
        {
            mResult = info;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write("\r\n>>> ");
            if (mState == EState.e_error) Console.ForegroundColor = ConsoleColor.Red;
            else if (mState == EState.e_warning) Console.ForegroundColor = ConsoleColor.Yellow;
            else if (mState == EState.e_normal) Console.ForegroundColor = Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(mResult);
            mResult = null;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("<<< ");
            Console.ForegroundColor = ConsoleColor.White;
        }

        void threadRun()
        {
            mExit = false;
            do
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("<<< ");
                Console.ForegroundColor = ConsoleColor.White;
                mScript = Console.ReadLine();
                while (mResult == null)
                {
                    Thread.Sleep(200);
                }
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write(">>> ");
                if (mState == EState.e_error) Console.ForegroundColor = ConsoleColor.Red;
                else if (mState == EState.e_warning) Console.ForegroundColor = ConsoleColor.Yellow;
                else if (mState == EState.e_normal) Console.ForegroundColor = Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine(mResult);
                mResult = null;
            } while (!mExit);
            mThread = null;
        }

        public void start()
        {
            init();
            mThread = new Thread(() => { threadRun(); });
            mThread.Start();
        }

        public void runOnce()
        {
            if (mScript != null)
            {
                var res = mCodeProvide.CompileAssemblyFromSource(mCompileOptions, String.Format(mCodeFrame, mScript));

                if (res.Errors.Count > 0)
                {
                    foreach (CompilerError error in res.Errors)
                    {
                        mResult = string.Format("Compiler Error ({0}): {1}", error.Line - 17, error.ErrorText);// 17 is location in whole codeFormat, 
                        mState = EState.e_error;
                    }
                }
                else
                {
                    var codeObject = res.CompiledAssembly.GetType("Wrapper");
                    var scriptFunc = codeObject.GetMethod("PerformAction", BindingFlags.Public | BindingFlags.Static);
                    if (scriptFunc != null)
                    {
                        try
                        {
                            mResult = "done";
                            scriptFunc.Invoke(null, null);
                        }
                        catch (Exception ex)
                        {
                            mResult = ex.ToString();
                        }

                        mState = EState.e_normal;
                    }
                    else
                    {
                        mResult = "runntime Error: scirptFunc == null";
                        mState = EState.e_error;
                    }
                }
                mScript = null;
            }
        }

        static public void stop()
        {
            mExit = true;
        }
    };
}
