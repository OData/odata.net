//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.Edm.Internal
{
    internal static class DependencyManager
    {
        public static void AddDependency(this IDependencyTrigger trigger, IDependent dependent)
        {
            trigger.Dependents.Add(dependent);
            dependent.DependsOn.Add(trigger);
        }

        public static HashSetInternal<IDependent> FireDependency(this IDependencyTrigger trigger)
        {
            var visitedDependents = new HashSetInternal<IDependent>();
            foreach (IDependent dependent in trigger.Dependents)
            {
                dependent.RespondToDependency(visitedDependents);
            }

            return visitedDependents;
        }

        private static void FireDependency(IDependencyTrigger trigger, HashSetInternal<IDependent> visitedDependents)
        {
            foreach (IDependent dependent in trigger.Dependents)
            {
                dependent.RespondToDependency(visitedDependents);
            }
        }

        public static void RespondToDependency(this IDependent dependent, HashSetInternal<IDependent> visitedDependents)
        {
            if (!visitedDependents.Contains(dependent))
            {
                visitedDependents.Add(dependent);
                dependent.FlushCaches();

                IDependencyTrigger dependentAsTrigger = dependent as IDependencyTrigger;
                if (dependentAsTrigger != null)
                {
                    FireDependency(dependentAsTrigger, visitedDependents);
                }
            }
        }

        private static HashSetInternal<IDependent> SetTriggerField<T>(IDependencyTrigger triggerContainingField, ref T field, T value)
        {
            field = value;

            var visitedDependents = triggerContainingField.FireDependency();

            IFlushCaches flushableTrigger = triggerContainingField as IFlushCaches;
            if (flushableTrigger != null)
            {
                flushableTrigger.FlushCaches();
            }

            return visitedDependents;
        }

        // SetField:
        //    1) Sets a field in a dependency trigger.
        //    2) Flushes caches of the transitive closure of all dependents of the trigger and of the trigger itself.
        //    3) If a dependent is supplied, flushes its caches and, if it is also a trigger, flushes the caches of the transitive closure of its dependents.
        //    4) If a dependent is supplied and the supplied value is a trigger, establishes a dependency from the dependent on the new value.
        //
        // SetTriggerField performs steps 1 and 2.
        // Step 3 is omitted if the dependent is the same object as the trigger.
        // Step 4 is omitted if the value is of a type that cannot be a trigger (e.g. bool or int).
        public static void SetField(this IDependencyTrigger triggerContainingField, ref bool field, bool value)
        {
            if (field != value)
            {
                SetTriggerField(triggerContainingField, ref field, value);
            }
        }

        public static void SetField(this IDependencyTrigger triggerContainingField, ref int field, int value)
        {
            if (field != value)
            {
                SetTriggerField(triggerContainingField, ref field, value);
            }
        }

        public static void SetField(this IDependencyTrigger triggerContainingField, ref string field, string value)
        {
            if (field != value)
            {
                SetTriggerField(triggerContainingField, ref field, value);
            }
        }

        public static void SetField<T>(this IDependencyTrigger triggerContainingField, ref T field, T value) where T : class
        {
            triggerContainingField.SetField(triggerContainingField as IDependent, ref field, value);
        }

        public static void SetField<T>(this IDependencyTrigger triggerContainingField, IDependent dependent, ref T field, T value) where T : class
        {
            if (field == value)
            {
                return;
            }

            HashSetInternal<IDependent> visitedDependents = SetTriggerField(triggerContainingField, ref field, value);

            if (dependent == null)
            {
                return;
            }

            if (dependent != triggerContainingField)
            {
                RespondToDependency(dependent, visitedDependents);
            }

            IDependencyTrigger valueAsTrigger = value as IDependencyTrigger;
            if (valueAsTrigger != null)
            {
                AddDependency(valueAsTrigger, dependent);
            }
        }
    }
}
