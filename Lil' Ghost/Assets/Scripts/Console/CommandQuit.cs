using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Console 
{
    public class CommandQuit : ConsoleCommand

    {
        public override string Name { get; protected set; }
        public override string Command { get; protected set; }
        public override string Description { get; protected set; }
        public override string Help { get; protected set; }

        public CommandQuit()
        {
            Name = "Quit";
            Command = "quit";
            Description = "Quits the Application";
            Help = "Use this command with no arguments to foce the appication to quit!";

            AddCommandToConsole();
        }

        public override void RunCommand(string[] args)
        {
            if(args.Length != 0)
            {
                Debug.LogWarning("this command does not take any arguments");
                return;
            }
            if (Application.isEditor)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }
            else
            {
                Application.Quit();
            }
            
        }


        public static CommandQuit CreateCommand()
        {
            return new CommandQuit();
        }
    }

}


