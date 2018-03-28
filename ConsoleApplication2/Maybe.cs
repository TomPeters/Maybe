using System;

namespace ConsoleApplication2
{
    public interface IMaybe<out T>
    {
        TReturn Bind<TReturn>(Func<T, TReturn> justHandler, Func<TReturn> nothingHandler);
    }

    public static class Maybe
    {
        public static IMaybe<T> Just<T>(T value)
        {
            return new JustImpl<T>(value);
        }

        public static IMaybe<T> Nothing<T>()
        {
            return new NothingImpl<T>();
        }

        private class JustImpl<T> : IMaybe<T>
        {
            private readonly T _value;

            public JustImpl(T value)
            {
                _value = value;
            }

            public TReturn Bind<TReturn>(Func<T, TReturn> justHandler, Func<TReturn> nothingHandler)
            {
                return justHandler(_value);
            }
        }

        private class NothingImpl<T> : IMaybe<T>
        {
            public TReturn Bind<TReturn>(Func<T, TReturn> justHandler, Func<TReturn> nothingHandler)
            {
                return nothingHandler();
            }
        }
        
        public static IMaybe<TReturn> Select<T, TReturn>(this IMaybe<T> maybe, Func<T, TReturn> selector)
        {
            return maybe.Bind(val => Just(selector(val)), Nothing<TReturn>);
        }

        public static void Do<T>(this IMaybe<T> maybe, Action<T> justHandler, Action nothingHandler)
        {
            maybe.Bind(val =>
            {
                justHandler(val);
                return 0;
            }, () =>
            {
                nothingHandler();
                return 0;
            });
        }

        public static void Do<T>(this IMaybe<T> maybe, Action<T> justHandler)
        {
            maybe.Bind(val =>
            {
                justHandler(val);
                return 0;
            }, () => 0);
        }

        public static bool HasValue<T>(this IMaybe<T> maybe)
        {
            return maybe.Bind(val => true, () => false);
        }

        public static T Or<T>(this IMaybe<T> maybe, T nullFallback)
        {
            return maybe.Bind(val => val, () => nullFallback);
        }

        public static IMaybe<T> If<T>(this IMaybe<T> maybe, Func<T, bool> predicate)
        {
            return maybe.Bind(val => predicate(val) ? Just(val) : new NothingImpl<T>(), Nothing<T>);
        }
    }
}
