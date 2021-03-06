﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

namespace Console 
{

    public abstract class ConsoleCommand
    {
        public abstract string Name { get; protected set; }
        public abstract string Command { get; protected set; }
        public abstract string Description { get; protected set; }
        public abstract string Help { get; protected set; }

        public void AddCommandToConsole()
        {
            string addMessage = " command has been added to consle.";

            DeveloperConsole.AddCommandsToConsole(Command, this);
            Debug.Log(Name + addMessage);
        }

        public abstract void RunCommand(string[] args);
    }


    public class DeveloperConsole : MonoBehaviour
    {
        public static DeveloperConsole Instance { get; private set; }
        public static Dictionary<string, ConsoleCommand> Commands { get; private set; }

        [Header("UI Components")]
        public Canvas consloeCanvas;
        public Text consoleText;
        public Text inputText;
        public InputField consoleInput;

        private void Awake()
        {
            if(Instance != null)
            {
                return;
            }
            Instance = this;
            Commands = new Dictionary<string, ConsoleCommand>();
        }

        private void Start()
        {
            consloeCanvas.gameObject.SetActive(false);
            CreateCommands();
        }

        private void OnEnable()
        {
            Application.logMessageReceived += HandleLog;
        }
        private void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
        }
        private void HandleLog(string logMessage, string stackTrace, LogType type)
        {
            string _message = "[" + type.ToString() + "] " + logMessage;
            AddMessageToConsole(_message);
        }

        private void CreateCommands()
        {
            CommandQuit.CreateCommand();
            CommandGoTo.CreateCommand();
        }

        public static void AddCommandsToConsole(string _name, ConsoleCommand _command) 
        {
            if (!Commands.ContainsKey(_name))
            {
                Commands.Add(_name, _command);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                consloeCanvas.gameObject.SetActive(!consloeCanvas.gameObject.activeInHierarchy);
            }

            if (consloeCanvas.gameObject.activeInHierarchy)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    if(inputText.text != "")
                    {
                        AddMessageToConsole(inputText.text);
                        ParseInput(inputText.text);
                        consoleInput.text = string.Empty;
                    }
                }
            }
        }

        private void AddMessageToConsole(string msg)
        {
            consoleText.text += msg + "\n";
        }
        private void ParseInput(string input)
        {
            string[] _input = input.Split(' ');
            
            if (_input.Length == 0 || _input == null)
            {
                Debug.LogWarning("Command Not Recognized");
                return;
            }
            string commandName = _input[0];
            string[] args = _input.Skip(1).ToArray();
            if (!Commands.ContainsKey(commandName))
            {
                Debug.LogWarning("Command Not Recognized");
            }
            else
            {
                ProcessCommand(commandName, args);
            }

        }
        private void ProcessCommand(string commandName, string[] args)
        {
            Commands[commandName].RunCommand(args);
        }
    }

}

