using System;
using System.Collections.Generic;
using System.IO;
using ToolBox.Validations;
using dein.tools;
using static HardHat.Program;
using static dein.tools.Paths;
using static Colorify.Colors;

namespace HardHat
{
    public static partial class Project
    {
        public static void List(ref List<Option> opts)
        {
            opts.Add(new Option { opt = "p", status = true, action = Project.Select });
            opts.Add(new Option { opt = "pf", status = false, action = Project.SelectFile });
            opts.Add(new Option { opt = "pi", status = false, action = Adb.Install });
            opts.Add(new Option { opt = "pd", status = false, action = Project.Duplicate });
            opts.Add(new Option { opt = "pp", status = false, action = Project.FilePath });
            opts.Add(new Option { opt = "pp>p", status = false, action = Project.CopyPath });
            opts.Add(new Option { opt = "pp>f", status = false, action = Project.CopyFilePath });
            opts.Add(new Option { opt = "pp>m", status = false, action = Project.CopyMappingPath });
            opts.Add(new Option { opt = "ps", status = false, action = BuildTools.SignerVerify });
            opts.Add(new Option { opt = "pv", status = false, action = BuildTools.Information });
        }

        public static void Status(string dirPath)
        {
            if (!_fileSystem.DirectoryExists(dirPath))
            {
                _config.personal.selected.project = "";
                _config.personal.selected.file = "";
            }
            Options.Valid("p", true);
            string selectedFile = _path.Combine(dirPath, _config.android.projectPath, _config.android.buildPath, _config.personal.selected.path, _config.personal.selected.file);
            if (!File.Exists(selectedFile))
            {
                _config.personal.selected.path = "";
                _config.personal.selected.file = "";
                _config.personal.selected.packageName = "";
                _config.personal.selected.mapping = "";
                _config.personal.selected.mappingStatus = false;
            }
            string selectedFileMapping = _path.Combine(dirPath, _config.android.projectPath, _config.android.buildPath, _config.personal.selected.path, _config.personal.selected.mapping);
            _config.personal.selected.mappingStatus = File.Exists(selectedFileMapping);
            Options.Valid("pf", !Strings.SomeNullOrEmpty(_config.personal.selected.project));
            Options.Valid("pi", !Strings.SomeNullOrEmpty(_config.personal.selected.project, _config.personal.selected.file));
            Options.Valid("pd", !Strings.SomeNullOrEmpty(_config.personal.selected.project, _config.personal.selected.file));
            Options.Valid("pp", !Strings.SomeNullOrEmpty(_config.personal.selected.project, _config.personal.selected.file));
            Options.Valid("pp>p", !Strings.SomeNullOrEmpty(_config.personal.selected.project, _config.personal.selected.file));
            Options.Valid("pp>f", !Strings.SomeNullOrEmpty(_config.personal.selected.project, _config.personal.selected.file));
            Options.Valid("pp>m", _config.personal.selected.mappingStatus);
            Options.Valid("ps", !Strings.SomeNullOrEmpty(_config.personal.selected.project, _config.personal.selected.file));
            Options.Valid("pv", !Strings.SomeNullOrEmpty(_config.personal.selected.project, _config.personal.selected.file));
        }

        public static void Start()
        {
            if (String.IsNullOrEmpty(_config.personal.selected.project))
            {
                _colorify.WriteLine($" [P] Select Project", txtPrimary);
            }
            else
            {
                _colorify.Write($" [P] Selected Project: ", txtPrimary);
                _colorify.WriteLine($"{_config.personal.selected.project}");
            }

            if (String.IsNullOrEmpty(_config.personal.selected.project))
            {
                _colorify.WriteLine($"   [F] Select File", txtStatus(Options.Valid("pf")));
            }
            else
            {
                _colorify.Write($"   [F] Selected File:  ", txtPrimary);
                _colorify.Write($"{_config.personal.selected.file}");
                string mappingStatus = (_config.personal.selected.mappingStatus ? "(M)" : "");
                _colorify.WriteLine($" {mappingStatus}", txtWarning);
            }

            _colorify.Write($"{"   [I] Install",-17}", txtStatus(Options.Valid("pi")));
            _colorify.Write($"{"[D] Duplicate",-17}", txtStatus(Options.Valid("pd")));
            _colorify.Write($"{"[P] Path",-17}", txtStatus(Options.Valid("pp")));
            _colorify.Write($"{"[S] Signer",-17}", txtStatus(Options.Valid("ps")));
            _colorify.WriteLine($"{"[V] Values",-17}", txtStatus(Options.Valid("pv")));

            _colorify.BlankLines();
        }

        public static void Select()
        {
            _colorify.Clear();

            try
            {
                Section.Header("SELECT PROJECT");

                string dirPath = _path.Combine(_config.path.development, _config.path.workspace, _config.path.project);
                dirPath.Exists("Please review your configuration file.");
                List<string> dirs = dirPath.Directories(_config.path.filter, "projects");

                if (dirs.Count < 1)
                {
                    _config.personal.selected.project = "";
                }
                else
                {
                    var i = 1;
                    foreach (var dir in dirs)
                    {
                        string d = dir;
                        _colorify.WriteLine($" {i,2}] {_path.GetFileName(d)}", txtPrimary);
                        i++;
                    }
                }

                _colorify.BlankLines();
                _colorify.WriteLine($"{"[EMPTY] Cancel",82}", txtDanger);

                Section.HorizontalRule();

                _colorify.Write($"{" Make your choice:",-25}", txtInfo);
                string opt = Console.ReadLine().Trim();

                if (!String.IsNullOrEmpty(opt))
                {
                    Number.IsOnRange(1, Convert.ToInt32(opt), dirs.Count);

                    var sel = dirs[Convert.ToInt32(opt) - 1];
                    _config.personal.selected.project = _path.GetFileName(sel);
                }

                Menu.Start();
            }
            catch (Exception Ex)
            {
                Exceptions.General(Ex);
            }
        }

        public static void SelectFile()
        {
            _colorify.Clear();

            try
            {
                Section.Header("PROJECT", "SELECT FILE");

                string dirPath = _path.Combine(_config.path.development, _config.path.workspace, _config.path.project, _config.personal.selected.project, _config.android.projectPath, _config.android.buildPath);
                dirPath.Exists("Please review your configuration file or make a build first.");
                List<string> files = dirPath.Files($"*{_config.android.buildExtension}", "Please make a build first.", SearchOption.AllDirectories);

                if (files.Count < 1)
                {
                    _config.personal.selected.path = "";
                    _config.personal.selected.file = "";
                    _config.personal.selected.packageName = "";
                }
                else
                {
                    var i = 1;
                    foreach (var file in files)
                    {
                        string f = file;
                        _colorify.WriteLine($" {i,2}] {_path.GetFileName(f)}", txtPrimary);
                        i++;
                    }
                }

                _colorify.BlankLines();
                _colorify.WriteLine($"{"[EMPTY] Cancel",82}", txtDanger);

                Section.HorizontalRule();

                _colorify.Write($"{" Make your choice:",-25}", txtInfo);
                string opt = Console.ReadLine().Trim();

                if (!String.IsNullOrEmpty(opt))
                {
                    Number.IsOnRange(1, Convert.ToInt32(opt), files.Count);
                    var sel = files[Convert.ToInt32(opt) - 1];
                    _config.personal.selected.path = _path.Split(_path.GetDirectoryName(sel), dirPath);
                    _config.personal.selected.file = _path.GetFileName(sel);
                    _config.personal.selected.mapping = _config.personal.selected.file.Replace(_config.android.buildExtension, _config.android.mappingSuffix);
                    _config.personal.selected.packageName = BuildTools.CmdGetPackageName(sel);
                }

                Menu.Start();
            }
            catch (Exception Ex)
            {
                Exceptions.General(Ex);
            }
        }
    }
}