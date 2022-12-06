using System.Runtime.InteropServices;
using static System.ArgumentNullException;

namespace MiscUtil;

public static class CollectionExtensions
{
    /// <summary>
    /// Efficiently gets the current value in the dictionary for the given key. If the key is not present,
    /// it is added with <paramref name="defaultValue"/> as the associated value.
    /// </summary>
    /// <typeparam name="TKey">The type of the dictionary key.</typeparam>
    /// <typeparam name="TValue">The type of the dictionary value.</typeparam>
    /// <param name="dictionary">The dictionary to modify.</param>
    /// <param name="key">The key to search for.</param>
    /// <param name="defaultValue">The default value to add if the key is not found.</param>
    /// <returns>Returns the current value or the value provided in <paramref name="defaultValue"/>.</returns>
    public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        where TKey : notnull
    {
        ThrowIfNull(dictionary);

        ref TValue? value = ref CollectionsMarshal.GetValueRefOrAddDefault(dictionary, key, out bool exists);
        if (!exists)
        {
            // At this point, a new slot in the dictionary for the given key has been allocated, and assigning to the ref var
            // populates the dictionary - no need to pay the cost of an extra hash calculation by calling Add.
            value = defaultValue;
        }

        return value!;
    }

    /// <summary>
    /// Efficiently gets the current value in the dictionary for the given key. If the key is not present,
    /// <paramref name="valueFactory"/> is called to provide a default value to add.
    /// </summary>
    /// <typeparam name="TKey">The type of the dictionary key.</typeparam>
    /// <typeparam name="TValue">The type of the dictionary value.</typeparam>
    /// <param name="dictionary">The dictionary to modify.</param>
    /// <param name="key">The key to search for.</param>
    /// <param name="valueFactory">A function that produces a default value for the given key, if required.</param>
    /// <returns>Returns the current value or the value provided by <paramref name="valueFactory"/>.</returns>
    public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> valueFactory)
        where TKey : notnull
    {
        ThrowIfNull(dictionary);
        ThrowIfNull(valueFactory);

        ref TValue? value = ref CollectionsMarshal.GetValueRefOrAddDefault(dictionary, key, out bool exists);
        if (!exists)
        {
            try
            {
                // At this point, a new slot in the dictionary for the given key has been allocated, and assigning to the ref var
                // populates the dictionary - no need to pay the cost of an extra hash calculation by calling Add.
                value = valueFactory(key);
            }
            catch (Exception)
            {
                // However, if the value factory throws an exception, we need to remove the empty entry
                // we just added, so as to not corrupt the dictonary's state.
                dictionary.Remove(key);
                throw;
            }
        }

        return value!;
    }

    /// <summary>
    /// Efficiently adds or updates the value associated with the given key in the dictionary.
    /// </summary>
    /// <typeparam name="TKey">The type of the dictionary key.</typeparam>
    /// <typeparam name="TValue">The type of the dictionary value.</typeparam>
    /// <param name="dictionary">The dictionary to modify.</param>
    /// <param name="key">The key to search for.</param>
    /// <param name="valueToAdd">The value to add if the key is not present.</param>
    /// <param name="updateFunction">The function that provides an updated value if the key is present.</param>
    /// <returns>Returns the new value in the dictionary.</returns>
    public static TValue AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dictionary,
                                                   TKey key, TValue valueToAdd,
                                                   Func<TKey, TValue, TValue> updateFunction)
        where TKey : notnull
    {
        ThrowIfNull(dictionary);
        ThrowIfNull(updateFunction);

        ref TValue? curValue = ref CollectionsMarshal.GetValueRefOrAddDefault(dictionary, key, out bool exists);
        curValue = exists ? updateFunction(key, curValue!) : valueToAdd;

        return curValue!;
    }

    /// <summary>
    /// Efficiently adds or updates the value associated with the given key in the dictionary.
    /// </summary>
    /// <typeparam name="TKey">The type of the dictionary key.</typeparam>
    /// <typeparam name="TValue">The type of the dictionary value.</typeparam>
    /// <param name="dictionary">The dictionary to modify.</param>
    /// <param name="key">The key to search for.</param>
    /// <param name="valueFactory">The function that provides a default value if the key is not present.</param>
    /// <param name="updateFunction">The function that provides an updated value if the key is present.</param>
    /// <returns>Returns the new value in the dictionary.</returns>
    public static TValue AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dictionary,
                                                   TKey key, Func<TKey, TValue> valueFactory,
                                                   Func<TKey, TValue, TValue> updateFunction)
        where TKey : notnull
    {
        ThrowIfNull(dictionary);
        ThrowIfNull(valueFactory);
        ThrowIfNull(updateFunction);

        ref TValue? curValue = ref CollectionsMarshal.GetValueRefOrAddDefault(dictionary, key, out bool exists);
        if (exists)
        {
            curValue = updateFunction(key, curValue!);
        }
        else
        {
            try
            {
                curValue = valueFactory(key);
            }
            catch (Exception)
            {
                // If the value factory throws an exception, we need to remove the empty entry
                // we just added, so as to not corrupt the dictonary's state.
                dictionary.Remove(key);
                throw;
            }
        }

        return curValue!;
    }

    /// <summary>
    /// Adds the elements of the specified collection to the end of the list.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    /// <param name="thisList">The list.</param>
    /// <param name="items">The items.</param>
    public static void AddRange<T>(this IList<T> thisList, IEnumerable<T> items)
    {
        ThrowIfNull(items);

        if (thisList is List<T> list)
        {
            list.AddRange(items);
        }
        else if (thisList is not null)
        {
            foreach (var item in items)
            {
                thisList.Add(item);
            }
        }
    }

    /// <summary>
    /// Removes all elements matching the given predicate.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="thisList">The list.</param>
    /// <param name="match">The predicate function that returns true for each item that should be removed.</param>
    public static void RemoveAll<T>(this IList<T> thisList, Predicate<T> match)
    {
        ThrowIfNull(match);

        if (thisList is List<T> list)
        {
            list.RemoveAll(match);
        }
        else if (thisList is not null)
        {
            int count = thisList.Count;
            List<int> deleteIndexes = new(count);
            for (int i = 0; i < count; i++)
            {
                if (match(thisList[i]))
                {
                    deleteIndexes.Add(i);
                }
            }

            for (int i = deleteIndexes.Count - 1; i >= 0; i--)
            {
                thisList.RemoveAt(deleteIndexes[i]);
            }
        }
    }
}
