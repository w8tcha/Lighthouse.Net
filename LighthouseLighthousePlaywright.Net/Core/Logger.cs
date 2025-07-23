using System;
using System.IO;
using System.Threading;

namespace Lighthouse.Net.Core;

internal sealed class Logger
{
    private readonly string _tempDirectory;
    private readonly string _fileName;
    private readonly Lock lockObj = new();

    internal Logger(string name)
    {
        this._tempDirectory = Path.GetTempPath();
        this._fileName = $"{name}-{Guid.NewGuid():N}.txt";
    }

    public bool Append(string content)
    {
        try
        {
            lock (lockObj)
            {
                File.AppendAllText(_tempDirectory + _fileName, content);
            }

            return true;
        }
        catch
        {
            // ignore
        }

        return false;
    }
}