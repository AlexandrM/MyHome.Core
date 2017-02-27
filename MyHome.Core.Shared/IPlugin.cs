using System;
using System.Threading.Tasks;

namespace MyHome.Shared
{
    delegate void Invoke(string s, params object[] args);

    public interface IPlugin
    {
        /// <summary>
        /// Runned on app start
        /// </summary>
        /// <returns></returns>
        Task<bool> Init();

        /// <summary>
        /// Runned every 1 minute
        /// </summary>
        /// <returns></returns>
        Task<bool> Process();

        /// <summary>
        /// Runed from web or from scheduler
        /// </summary>
        /// <param name="value"></param>
        /// <param name="oldValue"></param>
        /// <returns></returns>
        Task<bool> ChangeValue(ElementItemValueModel value, ElementItemValueModel oldValue);
    }

    public interface ISignalRProxy
    {
        void ChangeElementItemValue(ElementItemValueModel value);

        void TryChangeElementItemValue(ElementItemValueModel value);

        void AfterChangeElementItemValue(ElementItemValueModel value);
    }
}
