using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Threading.Tasks;

namespace BLRP_MULTIVEHICLE_LOCK_SERVER
{
    public class Main : BaseScript
    {
        public Main()
        {
            EventHandlers["BLRP_MULTILOCK:RequestUnlock"] += new Action<Player, int>(UnlockVehicle);
            EventHandlers["BLRP_MULTILOCK:RequestLock"] += new Action<Player, int>(LockVehicle);
        }

        private void UnlockVehicle([FromSource] Player player, int vehicleID)
        {
            TriggerClientEvent("BLRP_MULTILOCK:UnlockVehicle", vehicleID);
        }

        private void LockVehicle([FromSource] Player player, int vehicleID)
        {
            TriggerClientEvent("BLRP_MULTILOCK:LockVehicle", vehicleID);
        }
    }
}
