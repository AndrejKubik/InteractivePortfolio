using System;
using System.IO;

namespace Snek.Utilities
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SnekAutoGenerateAssetAttribute : Attribute
    {
        public string TargetAssetPath { get; private set; }

        public SnekAutoGenerateAssetAttribute(string folderPath, string fileName)
        {
            TargetAssetPath = Path.Combine(folderPath, $"{fileName}.asset");
        }

        public SnekAutoGenerateAssetAttribute(string targetAssetPath)
        {
            TargetAssetPath = targetAssetPath;
        }
    }
}
