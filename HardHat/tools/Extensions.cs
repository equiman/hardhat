using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Colorify;
using static Colorify.Colors;
using ToolBox.Platform;
using ToolBox.System;
using ToolBox.Transform;
using static HardHat.Program;

namespace dein.tools
{
    public static class Paths
    {
        public static void Exists(this string dir, string message = null)
        {
            try
            {
                if (!Directory.Exists(dir)){
                    StringBuilder msg = new StringBuilder();
                    msg.Append($" Path not found:{Environment.NewLine}");
                    msg.Append($" '{dir}'{Environment.NewLine}");
                    if (!String.IsNullOrEmpty(message))
                    {
                        msg.Append(Environment.NewLine);
                        msg.Append($" {message}");
                    }

                    Message.Error(
                        msg: msg.ToString()
                    );
                }
            }
            catch (Exception Ex){
                Exceptions.General(Ex.Message);
            }
        }

        public static List<string> Directories(this string dir, string flt, string type){
            List<string> dirs = new List<string>();
            try
            {
                dirs = new List<string>(Directory.EnumerateDirectories(dir, flt).OrderBy(name => name));
                if (dirs.Count < 1)
                {
                    StringBuilder msg = new StringBuilder();
                    msg.Append($" There is no {type} in current location:{Environment.NewLine}");
                    msg.Append($" '{dir}'");
                    
                    Message.Alert(msg.ToString());
                }
            }
            catch (UnauthorizedAccessException UAEx)
            {
                Exceptions.General(UAEx.Message);
            }
            catch (PathTooLongException PathEx)
            {
                Exceptions.General(PathEx.Message);
            }
            catch (Exception Ex){
                Exceptions.General(Ex.Message);
            }
            return dirs;
        }
    
        public static List<string> Files(this string dir, string flt, string message = null){
            List<string> files = new List<string>();
            try
            {
                files = new List<string>(Directory.EnumerateFiles(dir, flt).OrderBy(name => name));
                if (files.Count < 1)
                {
                    StringBuilder msg = new StringBuilder();
                    msg.Append($" There is no files in current location.{Environment.NewLine}");
                    msg.Append($" '{dir}'{Environment.NewLine}");
                    if (!String.IsNullOrEmpty(message))
                    {
                        msg.Append(Environment.NewLine);
                        msg.Append($" {message}");
                    }

                    Message.Alert(msg.ToString());
                }
            }
            catch (UnauthorizedAccessException UAEx)
            {
                Exceptions.General(UAEx.Message);
            }
            catch (PathTooLongException PathEx)
            {
                Exceptions.General(PathEx.Message);
            }
            catch (Exception Ex){
                Exceptions.General(Ex.Message);
            }
            return files;
        }

        private static bool isFiltered(List<string> filter, string file)
        {
            bool valid = false;
            try
            {
                if (filter == null)
                {
                    valid = true;
                } else 
                {
                    valid = filter.Any(f => file.EndsWith(f, StringComparison.OrdinalIgnoreCase));
                }
            }
            catch (Exception Ex){
                Exceptions.General(Ex.Message);
            }
            return valid;
        }

        public static void CopyAll(string sourcePath, string destinationPath, bool overWrite = false, bool show = false, List<string> filter = null)
        {
            try
            {
                string[] directories = Directory
                    .GetDirectories(sourcePath, "*.*", SearchOption.AllDirectories);
                Parallel.ForEach(directories, dirPath =>
                {
                    _colorify.Wrap($" [COPY] {Strings.RemoveWords(dirPath,sourcePath)}", txtPrimary);
                    Directory.CreateDirectory(dirPath.Replace(sourcePath, destinationPath));
                }); 

                var files = Directory
                    .GetFiles(sourcePath, "*.*", SearchOption.AllDirectories)
                    .Where(f => isFiltered(filter, f));
                Parallel.ForEach(files, newPath =>
                {
                    _colorify.Wrap($"  [COPY] {Strings.RemoveWords(newPath,sourcePath)}", txtPrimary);
                    File.Copy(newPath, newPath.Replace(sourcePath, destinationPath), overWrite);
                }); 
            }
            catch (UnauthorizedAccessException UAEx)
            {
                Exceptions.General(UAEx.Message);
            }
            catch (PathTooLongException PathEx)
            {
                Exceptions.General(PathEx.Message);
            }
            catch (Exception Ex){
                Exceptions.General(Ex.Message);
            }
        }

        public static void DeleteAll(string sourcePath, bool recursive, bool show = false)
        {
            try
            {
                _colorify.Wrap($"  [DEL] {sourcePath}", txtPrimary);
                Directory.Delete(sourcePath, recursive);
            }
            catch (DirectoryNotFoundException) 
            {
                return;  // good!
            }
            catch (UnauthorizedAccessException UAEx)
            {
                Exceptions.General(UAEx.Message);
            }
            catch (PathTooLongException PathEx)
            {
                Exceptions.General(PathEx.Message);
            }
            catch (Exception Ex){
                Exceptions.General(Ex.Message);
            }
        }
    }
}