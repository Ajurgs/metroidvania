using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Console
{


    public class CommandGoTo : ConsoleCommand
    {
        public override string Name { get; protected set; }
        public override string Command { get; protected set; }
        public override string Description { get; protected set; }
        public override string Help { get; protected set; }


        public CommandGoTo()
        {
            Name = "GoTo";
            Command = "goto";
            Description = "moves player to the input room";
            Help = "this command takes one integer argument and will load the desired room and move player to the room";

            AddCommandToConsole();
        }
        public override void RunCommand(string[] args)
        {
            if( args.Length != 1)
            {
                Debug.LogWarning("this command must have one and only one argument");
                return;
            }
            if(!int.TryParse(args[0],out int value))
            {
                Debug.LogWarning("input argument must be an integer");
                return;
            }
            if(GameControl.control.currentScene == 0)
            {
                Debug.LogWarning("Can not be in the Main Menu to use this command");
                return;
            }

            GameControl.control.commandChangeRoom = true;

            GameControl.control.LoadScene(value);
        }

        public static CommandGoTo CreateCommand()
        {
            return new CommandGoTo();
        }
    }

}