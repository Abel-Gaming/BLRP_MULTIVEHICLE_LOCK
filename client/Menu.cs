using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using NativeUI;
using System;
using System.Collections.Generic;

namespace BLRP_MULTIVEHICLE_LOCK
{
    public class Menu : BaseScript
    {
        private static MenuPool _menuPool;
        private static UIMenu mainMenu;

        private static List<dynamic> intList;
        private static List<dynamic> ModelList;

        private static int vehicleID;

        public void MenuOptions(UIMenu menu)
        {
            //Create list
            intList = new List<dynamic>();
            ModelList = new List<dynamic>();

            //Add placeholders
            intList.Add("No Ints");
            ModelList.Add("No vehicles locked");

            //Create NativeUI List Items
            var intListItem = new UIMenuListItem("Vehicle INT", intList, 0);
            var ModelListItem = new UIMenuListItem("Vehicle", ModelList, 0);

            //Add vehicle item
            var addVehicle = new UIMenuItem("Add Vehicle To List");
            menu.AddItem(addVehicle);
            menu.OnItemSelect += (sender, item, index) =>
            {
                if (item == addVehicle)
                {
                    if (Game.Player.Character.IsInVehicle())
                    {
                        if (intListItem.Items[0].ToString() == "No Ints")
                        {
                            intList.Clear();
                            ModelList.Clear();
                        }
                        intList.Add(Game.Player.Character.CurrentVehicle.Handle);
                        ModelList.Add(Game.Player.Character.CurrentVehicle.DisplayName);
                        TriggerEvent("swt_notifications:Icon", "The vehicle has been added!", "bottom", 3000, "green-13", "grey-10", false, "mdi-car");
                        //Screen.ShowNotification("~g~[Success]~w~ The vehicle has been added!");
                        _menuPool.CloseAllMenus();
                    }
                    else
                    {
                        Screen.ShowNotification("~r~[ERROR]~w~ You are not in a vehicle");
                    }
                }
            };

            //Add List Item
            menu.AddItem(ModelListItem);

            //List Change
            menu.OnListChange += (sender, item, index) =>
            {
                if (item == ModelListItem)
                {
                    intListItem.Index = ModelListItem.Index;
                }
            };

            //List Select
            menu.OnListSelect += (sender, item, index) =>
            {
                if (item == ModelListItem)
                {
                    vehicleID = int.Parse(intListItem.Items[index].ToString());
                    string vehicle = ModelListItem.Items[index].ToString();
                    TriggerEvent("swt_notifications:Icon", $"You selected {vehicle}", "bottom", 3000, "grey-9", "grey-1", false, "mdi-car");
                    //Screen.ShowNotification($"Selected Vehicle: ~b~{vehicle}");
                }
            };

            //Create lock/unlock items
            var unlock = new UIMenuItem("Unlock Selected Vehicle");
            var lockcar = new UIMenuItem("Lock Selected Vehicle");

            //Add Unlock/Lock Items
            menu.AddItem(unlock);
            menu.AddItem(lockcar);

            //Menu Select
            menu.OnItemSelect += (sender, item, index) =>
            {
                if (item == unlock)
                {
                    TriggerServerEvent("BLRP_MULTILOCK:RequestUnlock", vehicleID);
                    TriggerEvent("swt_notifications:Success", "Success", "Vehicle has been unlocked!", "bottom", 3000, false);
                }
                else if (item == lockcar)
                {
                    TriggerServerEvent("BLRP_MULTILOCK:RequestLock", vehicleID);
                    TriggerEvent("swt_notifications:Success", "Success", "Vehicle has been locked!", "bottom", 3000, false);
                }
            };
        }

        public Menu()
        {
            //Events
            EventHandlers["BLRP_MULTILOCK:UnlockVehicle"] += new Action<int>(UnlockVehicle);
            EventHandlers["BLRP_MULTILOCK:LockVehicle"] += new Action<int>(LockVehicle);

            //Commands
            API.RegisterCommand("+multilock", new Action(OpenMenu), false);
            API.RegisterCommand("-multilock", new Action(CloseMenu), false);

            //Key Mapping
            API.RegisterKeyMapping("+multilock", "Open Multi-Vehicle Lock Menu", "KEYBOARD", "F7");

            //Menu Items
            _menuPool = new MenuPool();
            mainMenu = new UIMenu("Multi-Vehicle Lock", "BlueLine Framework");
            _menuPool.Add(mainMenu);

            //Add items to menu
            MenuOptions(mainMenu);

            //Menu Pool
            _menuPool.MouseEdgeEnabled = false;
            _menuPool.ControlDisablingEnabled = false;
            _menuPool.RefreshIndex();

            //Tick
            Tick += async () =>
            {
                _menuPool.ProcessMenus();

                //Disable Combat
                if (_menuPool.IsAnyMenuOpen())
                {
                    Game.DisableControlThisFrame(0, Control.MeleeAttackLight);
                    Game.DisableControlThisFrame(0, Control.MeleeAttack1);
                }
                else
                {
                    Game.EnableControlThisFrame(0, Control.MeleeAttackLight);
                    Game.EnableControlThisFrame(0, Control.MeleeAttack1);
                }
            };
        }

        private static void OpenMenu()
        {
            mainMenu.Visible = !mainMenu.Visible;
        }

        private static void CloseMenu()
        {
            
        }

        private static void UnlockVehicle(int vehicleID)
        {
            API.SetVehicleDoorsLocked(vehicleID, 1);
        }

        private static void LockVehicle(int vehicleID)
        {
            API.SetVehicleDoorsLocked(vehicleID, 2);
        }
    }
}
