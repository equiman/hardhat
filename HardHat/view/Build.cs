using System;
using System.Collections.Generic;
using System.Text;
using ToolBox.Validations;
using dein.tools;
using static HardHat.Program;
using static Colorify.Colors;

namespace HardHat
{
    public static partial class Build
    {

        public static void List(ref List<Option> opts)
        {
            opts.Add(new Option { opt = "b", status = false, action = Build.Select });
            opts.Add(new Option { opt = "b>d", status = false, action = Build.Dimension });
            opts.Add(new Option { opt = "b>f", status = false, action = Build.Flavor });
            opts.Add(new Option { opt = "b>m", status = false, action = Build.Mode });
            opts.Add(new Option { opt = "bp", status = false, action = Build.Properties });
            opts.Add(new Option { opt = "bc", status = false, action = Build.Clean });
            opts.Add(new Option { opt = "bg", status = false, action = Build.Gradle });
        }

        public static void Status()
        {
            StringBuilder buildConfiguration = new StringBuilder();
            buildConfiguration.Append(_config.personal.gradle.dimension ?? "");
            buildConfiguration.Append(Selector.Name(Selector.Flavor, _config.personal.gradle.flavor));
            buildConfiguration.Append(Selector.Name(Selector.Mode, _config.personal.gradle.mode));
            _config.personal.menu.buildConfiguration = buildConfiguration.ToString();
            _config.personal.menu.buildValidation = !Strings.SomeNullOrEmpty(_config.personal.selected.project, _config.personal.gradle.mode, _config.personal.gradle.flavor, _config.personal.menu.buildConfiguration);
            Options.Valid("b", Variables.Valid("gh"));
            Options.Valid("b>d", Variables.Valid("gh"));
            Options.Valid("b>f", Variables.Valid("gh"));
            Options.Valid("b>m", Variables.Valid("gh"));
            Options.Valid("bp", Variables.Valid("gp") && !Strings.SomeNullOrEmpty(_config.personal.selected.project));
            Options.Valid("bc", Variables.Valid("gh") && !Strings.SomeNullOrEmpty(_config.personal.selected.project));
            Options.Valid("bg", Variables.Valid("gh") && _config.personal.menu.buildValidation);
        }

        public static void Start()
        {
            if (String.IsNullOrEmpty(_config.personal.menu.buildConfiguration))
            {
                _colorify.WriteLine($" [B] Build", txtStatus(Options.Valid("b")));
            }
            else
            {
                _colorify.Write($" [B] Build: ", txtStatus(Options.Valid("b")));
                Section.Configuration(_config.personal.menu.buildValidation, _config.personal.menu.buildConfiguration);
            }
            _colorify.Write($"{"   [P] Properties",-34}", txtStatus(Options.Valid("bp")));
            _colorify.Write($"{"[C] Clean",-34}", txtStatus(Options.Valid("bc")));
            _colorify.WriteLine($"{"[G] Gradle",-17}", txtStatus(Options.Valid("bg")));
            _colorify.BlankLines();
        }

        public static void Select()
        {
            _colorify.Clear();

            try
            {
                Section.Header("BUILD CONFIGURATION");
                Section.SelectedProject();
                Section.CurrentConfiguration(_config.personal.menu.buildValidation, _config.personal.menu.buildConfiguration);

                _colorify.BlankLines();
                _colorify.Write($"{" [D] Dimension:",-25}", txtPrimary); _colorify.WriteLine($"{_config.personal.gradle.dimension}");
                string buildFlavor = Selector.Name(Selector.Flavor, _config.personal.gradle.flavor);
                _colorify.Write($"{" [F] Flavor:",-25}", txtPrimary); _colorify.WriteLine($"{buildFlavor}");
                string buildMode = Selector.Name(Selector.Mode, _config.personal.gradle.mode);
                _colorify.Write($"{" [M] Mode:",-25}", txtPrimary); _colorify.WriteLine($"{buildMode}");

                _colorify.WriteLine($"{"[EMPTY] Exit",82}", txtDanger);

                Section.HorizontalRule();

                _colorify.Write($"{" Make your choice:",-25}", txtInfo);
                string opt = Console.ReadLine()?.ToLower();

                if (String.IsNullOrEmpty(opt))
                {
                    Menu.Start();
                }
                else
                {
                    Menu.Route($"b>{opt}", "b");
                }
                Message.Error();
            }
            catch (Exception Ex)
            {
                Exceptions.General(Ex);
            }
        }

        public static void Dimension()
        {
            _colorify.Clear();

            try
            {
                Section.Header("BUILD CONFIGURATION", "DIMENSION");
                Section.SelectedProject();
                Section.CurrentConfiguration(_config.personal.menu.buildValidation, _config.personal.menu.buildConfiguration);

                _colorify.BlankLines();
                _colorify.WriteLine($" Write a project dimension:", txtPrimary);

                _colorify.BlankLines();
                _colorify.WriteLine($"{"[EMPTY] Remove",82}", txtWarning);

                Section.HorizontalRule();

                _colorify.Write($"{" Make your choice: ",-25}", txtInfo);
                string opt = Console.ReadLine().Trim();
                if (!String.IsNullOrEmpty(opt))
                {
                    _config.personal.gradle.dimension = $"{opt}";
                }
                else
                {
                    _config.personal.gradle.dimension = $"";
                }

                Menu.Status();
                Select();
            }
            catch (Exception Ex)
            {
                Exceptions.General(Ex);
            }
        }

        public static void Flavor()
        {
            _colorify.Clear();

            try
            {
                Section.Header("BUILD CONFIGURATION", "FLAVOR");
                Section.SelectedProject();
                Section.CurrentConfiguration(_config.personal.menu.buildValidation, _config.personal.menu.buildConfiguration);

                _config.personal.gradle.flavor = Selector.Start(Selector.Flavor, "a");

                Menu.Status();
                Select();
            }
            catch (Exception Ex)
            {
                Exceptions.General(Ex);
            }
        }

        public static void Mode()
        {
            _colorify.Clear();

            try
            {
                Section.Header("BUILD CONFIGURATION", "MODE");
                Section.SelectedProject();
                Section.CurrentConfiguration(_config.personal.menu.buildValidation, _config.personal.menu.buildConfiguration);

                _config.personal.gradle.mode = Selector.Start(Selector.Mode, "d");

                Menu.Status();
                Select();
            }
            catch (Exception Ex)
            {
                Exceptions.General(Ex);
            }
        }

        public static void Gradle()
        {
            _colorify.Clear();

            try
            {
                Vpn.Verification();

                string dirPath = _path.Combine(_config.path.development, _config.path.workspace, _config.path.project, _config.personal.selected.project, _config.android.projectPath);
                CmdGradle(dirPath, _config.personal.menu.buildConfiguration);

                Menu.Start();
            }
            catch (Exception Ex)
            {
                Exceptions.General(Ex);
            }
        }

        public static void Clean()
        {
            _colorify.Clear();

            try
            {
                Vpn.Verification();

                string dirPath = _path.Combine(_config.path.development, _config.path.workspace, _config.path.project, _config.personal.selected.project, _config.android.projectPath);
                CmdClean(dirPath);

                Menu.Start();
            }
            catch (Exception Ex)
            {
                Exceptions.General(Ex);
            }
        }

        public static void Properties()
        {
            _colorify.Clear();

            try
            {
                Section.Header("BUILD CONFIGURATION", "PROPERTIES");
                Section.SelectedProject();
                Section.CurrentConfiguration(_config.personal.menu.buildValidation, _config.personal.menu.buildConfiguration);

                string sourcePath = propertiesSource();
                string destinationPath = _path.Combine(_config.path.development, _config.path.workspace, _config.path.project, _config.personal.selected.project, _config.android.projectPath);

                _colorify.BlankLines();
                List<string> filter = _disk.FilterCreator(true, ".properties");

                _colorify.WriteLine($" --> Copying...", txtInfo);
                _colorify.BlankLines();
                _colorify.Write($"{" From:",-8}", txtMuted); _colorify.WriteLine($"{sourcePath}");
                _colorify.Write($"{" To:",-8}", txtMuted); _colorify.WriteLine($"{destinationPath}");
                _disk.CopyAll(sourcePath, destinationPath, true, filter);

                Section.HorizontalRule();
                Section.Pause();

                Menu.Start();
            }
            catch (Exception Ex)
            {
                Exceptions.General(Ex);
            }
        }

        private static string propertiesSource()
        {
            string sourcePath = _path.Combine(Variables.Value("bp"));
            string bussinessPath = _path.Combine(sourcePath, _config.path.workspace);
            if (String.IsNullOrEmpty(_config.personal.gradle.dimension))
            {
                sourcePath = bussinessPath;
            }
            else
            {
                string sourceDimensionPath = _path.Combine(sourcePath, $"{_config.path.workspace}_{_config.personal.gradle.dimension}");
                if (_fileSystem.DirectoryExists(sourceDimensionPath))
                {
                    sourcePath = sourceDimensionPath;
                }
                else
                {
                    sourcePath = bussinessPath;
                }
            }
            return sourcePath;
        }
    }
}