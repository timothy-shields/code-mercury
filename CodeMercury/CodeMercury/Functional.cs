using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeMercury
{
    public static class Functional
    {
        public static T CastTo<T>(this object _this)
        {
            return (T)_this;
        }

        public static T As<T>(this object _this)
            where T : class
        {
            return _this as T;
        }

        public static bool Is<T>(this object _this)
        {
            return _this is T;
        }

        public static void Apply(Action _action)
        {
            _action();
        }

        public static T Apply<T>(Func<T> _func)
        {
            return _func();
        }

        public static void Using<TDisposable>(TDisposable _disposable, Action<TDisposable> _using)
            where TDisposable : IDisposable
        {
            using (_disposable) { _using(_disposable); }
        }

        public static T Using<TDisposable, T>(TDisposable _disposable, Func<TDisposable, T> _using)
            where TDisposable : IDisposable
        {
            using (_disposable) { return _using(_disposable); }
        }

        public static void Try(Action _try)
        {
            try { _try(); }
            catch { }
        }

        public static T Try<T>(Func<T> _try)
        {
            try { return _try(); }
            catch { return default(T); }
        }

        public static T Try<T>(Func<T> _try, T _catch)
        {
            try { return _try(); }
            catch { return _catch; }
        }

        public static void TryCatch(Action _try, Action _catch)
        {
            try { _try(); }
            catch { _catch(); }
        }

        public static void TryCatch<TException>(Action _try, Action<TException> _catch)
            where TException : Exception
        {
            try { _try(); }
            catch (TException e) { _catch(e); }
        }

        public static T TryCatch<T>(Func<T> _try, Func<T> _catch)
        {
            try { return _try(); }
            catch { return _catch(); }
        }

        public static T TryCatch<T, TException>(Func<T> _try, Func<TException, T> _catch)
            where TException : Exception
        {
            try { return _try(); }
            catch (TException e) { return _catch(e); }
        }
    }
}
