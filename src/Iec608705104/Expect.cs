namespace Iec608705104
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Collection of utility methods to check method arguments while claiming as little as possible of vertical screen space.
    /// Improves readability quite a bit.
    /// All methods throw <see cref="ArgumentException"/> or its derivatives.
    /// </summary>
    internal static class Expect
    {
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotNull(object arg, string argName)
        {
            if (arg == null)
            {
                throw new ArgumentNullException(argName);
            }
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotEmpty<T>(ICollection<T> arg, string argName)
        {
            if (arg == null)
            {
                throw new ArgumentNullException(argName);
            }

            if (arg.Count == 0)
            {
                throw new ArgumentException($"argument '{argName}' must not be empty {arg.GetType().Name}", argName);
            }
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Positive(int arg, string argName)
        {
            if (arg < 1)
            {
                throw new ArgumentException($"argument '{argName}' must be greater than 0", argName);
            }
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotDefault<T>(T arg, string argName)
        {
            if (arg.Equals(default(T)))
            {
                throw new ArgumentException($"argument '{argName}' must not equal default value for {arg.GetType().Name}", argName);
            }
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Range<T>(T arg, T min, T max, string argName)
            where T : IComparable<T>
        {
            if (arg.CompareTo(min) == -1 || arg.CompareTo(max) == 1)
            {
                throw new ArgumentException($"argument '{argName}' must within between {min} and {max} (both inclusive)", argName);
            }
        }

        [DebuggerStepThrough]
        public static void NotDisposed(bool isDisposed, [CallerMemberName] string callerMemberName = "")
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException($"calling method '{callerMemberName}' of disposed object is not allowed");
            }
        }
    }
}
