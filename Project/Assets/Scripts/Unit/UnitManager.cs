using UnityEngine;
using System.Linq;
using System.Collections.Generic;


namespace Gem
{
    public class UnitManager
    {
        #region SINGLETON
        private static UnitManager s_Instance = null;
        public static void Initialize()
        {
            s_Instance = new UnitManager();
        }
        public static void Destroy()
        {
            s_Instance = null;
        }
        #endregion

        private List<Unit> m_Units = new List<Unit>();
        private UniqueNumberGenerator m_IDGenerator = new UniqueNumberGenerator(0);

        private UnitManager()
        {

        }
        public void Update()
        {

        }
        public void CoroutineUpdate()
        {

        }

        public static void Register(Unit aUnit)
        {
            if(s_Instance != null && aUnit != null)
            {
                if(!s_Instance.m_Units.Contains(aUnit))
                {
                    s_Instance.m_Units.Add(aUnit);
                    aUnit.unitID = s_Instance.m_IDGenerator.Get();
                    Debug.Log("Registering " + aUnit.unitName);
                }
            }
        }

        public static void Unregister(Unit aUnit)
        {
            if(s_Instance != null && aUnit != null)
            {
                if(s_Instance.m_Units.Remove(aUnit))
                {
                    s_Instance.m_IDGenerator.Free(aUnit.unitID);
                }
            }
        }

        /// <summary>
        /// Sends an order to a unit who has the right name
        /// </summary>
        /// <param name="aName">The name of the unit to issue an order to</param>
        /// <param name="aOrder">The order to give to the unit</param>
        public static void IssueOrder(string aName, UnitOrderParams aOrder)
        {
            Unit unit = GetUnit(aName);
            if(unit != null && aOrder != null)
            {
                unit.ReceiveOrder(aOrder);
            }
        }
        /// <summary>
        /// Sends an order to a unit who has the right name && type
        /// </summary>
        /// <param name="aName">The name of the unit to issue an order to.</param>
        /// <param name="aUnitType">The type constraint of the unit</param>
        /// <param name="aOrder">The order to give to the unit</param>
        public static void IssueOrder(string aName, UnitType aUnitType, UnitOrderParams aOrder)
        {
            Unit unit = GetUnit(aName,aUnitType);
            if (unit != null && aOrder != null)
            {
                unit.ReceiveOrder(aOrder);
            }
        }
        /// <summary>
        /// Sends an order to a unit who has the right ID
        /// </summary>
        /// <param name="aID">The ID of the unit</param>
        /// <param name="aOrder">The order to issue the unit</param>
        public static void IssueOrder(int aID, UnitOrderParams aOrder)
        {
            Unit unit = GetUnit(aID);
            if (unit != null && aOrder != null)
            {
                unit.ReceiveOrder(aOrder);
            }
        }

        /// <summary>
        /// Gets a unit based on the name
        /// </summary>
        /// <param name="aName">The name of the unit</param>
        /// <returns></returns>
        public static Unit GetUnit(string aName)
        {
            if(s_Instance == null)
            {
                return null;
            }
            IEnumerator<Unit> iter = s_Instance.m_Units.GetEnumerator();
            while(iter.MoveNext())
            {
                if(iter.Current == null)
                {
                    continue;
                }
                if(iter.Current.unitName == aName)
                {
                    return iter.Current;
                }
            }
            return null;
        }
        /// <summary>
        /// Gets a unit based on name with a type constraint
        /// </summary>
        /// <param name="aName">The name of the unit</param>
        /// <param name="aType">The type constraint</param>
        /// <returns></returns>
        public static Unit GetUnit(string aName, UnitType aType)
        {
            if (s_Instance == null)
            {
                return null;
            }
            IEnumerator<Unit> iter = s_Instance.m_Units.GetEnumerator();
            while (iter.MoveNext())
            {
                if (iter.Current == null)
                {
                    continue;
                }
                if (iter.Current.unitName == aName && iter.Current.unitType == aType)
                {
                    return iter.Current;
                }
            }
            return null;
        }
        /// <summary>
        /// Gets a unit based on the ID 
        /// </summary>
        /// <param name="aID">The ID of the unit to search for</param>
        /// <returns></returns>
        public static Unit GetUnit(int aID)
        {
            if (s_Instance == null)
            {
                return null;
            }
            IEnumerator<Unit> iter = s_Instance.m_Units.GetEnumerator();
            while (iter.MoveNext())
            {
                if (iter.Current == null)
                {
                    continue;
                }
                if (iter.Current.unitID == aID)
                {
                    return iter.Current;
                }
            }
            return null;
        }
        /// <summary>
        /// Gets all units whos type matches the given type
        /// </summary>
        /// <param name="aType"></param>
        /// <returns></returns>
        public static IEnumerable<Unit> GetUnits(UnitType aType)
        {
            if(s_Instance == null)
            {
                return null;
            }
            return s_Instance.m_Units.Where(Element => Element.unitType == aType);
        }

        public static IEnumerable<Unit> GetAllUnits()
        {
            if(s_Instance != null)
            {
                return s_Instance.m_Units;
            }
            return null;
        }
    }
}