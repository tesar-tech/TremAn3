using System;
using System.Collections.Generic;
using System.Text;

namespace TremAn3.Core
{
    public class FftSettingsException : Exception
    {
        public FftSettingsException(string message) : base(message)
        {
        }
    }

    public class WindowLongerThanSignalException : FftSettingsException
    {
        public WindowLongerThanSignalException(int windowSizeSamples, int length) : base($"Window size for fft ({windowSizeSamples}) cannot be larger than signal size ({length}).")
        {
        }
    }

    public class WindowLowerThanOneException : FftSettingsException
    {
        public WindowLowerThanOneException():base("Size of window cannot be less than or equal to zero")
        {
        }
    }
    public class StepLowerThanOneException : FftSettingsException
    {
        public StepLowerThanOneException() : base("Step cannot be less than or equal to zero")
        {
        }
    }
}
