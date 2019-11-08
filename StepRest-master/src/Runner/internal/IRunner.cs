using System;
using System.Collections.Generic;
using System.Text;

namespace StepRest.Runner
{

    public interface IRunner
    {
        /// <summary>
        /// Refresh returns a new instance of the desired implimentation of IRunner
        /// </summary>
        /// <returns></returns>
        internal IRunner Refresh();

        /// <summary>
        /// Performs actions to save the background data.
        /// </summary>
        internal void SaveBackground();
    }
}
