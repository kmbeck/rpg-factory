using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMenu_Player : MonoBehaviour
{
    public GameObject player;

    void OnGUI() {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        float pad = 10f;
        float lineHeight = 20f;
        PlayerController pc = player.GetComponent<PlayerController>();

        // Container for elements.
        GUI.Box(new Rect(0,0,screenWidth,screenHeight),"Player");
        
        /* * * * * * * * * *
         * EQUIPMENT SECTION (ON THE LEFT)
         * * * * * * * * * */
        // Header for Equipment Section.
        GUI.Label(new Rect(pad,lineHeight*2,screenWidth/2f - pad/2f,24f),"Equipment");

        // Display Equip Slots & Equipped Items.
        for(int i = 0; i < pc.em.layout.slots.Count; i++) {
            // Make a label displaying current equipment.
            ItemEquipment slotItem = pc.em.getItemInSlot(pc.em.layout.slots[i]);
            string slotItemName = "NONE";
            if (slotItem != null) { 
                slotItemName = slotItem.data.displayName;
            }
            GUI.Label(
                new Rect(pad * 2, lineHeight*(3+i),200f,24f),
                pc.em.layout.slots[i].ToString()
            );
            GUI.Label(
                new Rect(150f + pad * 2, lineHeight*(3+i),200f,24f),
                slotItemName
            );
            // Unequip Buttons.
            if (slotItem != null) {
                if (GUI.Button(new Rect(300f + pad, lineHeight*(3+i),24f,24f),"R")) {
                    pc.em.unequip(slotItem);
                }
            }
        }
        
        float currentLineY = lineHeight * (3 + pc.em.layout.slots.Count + 1);

        // Equpment Scroll View Label
        GUI.Label(new Rect(pad,currentLineY,screenWidth/2f - pad/2f,24f),"Equipment Library:");

        // Scroll View of all Equipment...
        Vector2 equipScrollView = Vector2.zero;
        equipScrollView = GUI.BeginScrollView(
            new Rect(pad * 2, currentLineY + lineHeight * 2, 200f, 300f), 
            equipScrollView, 
            new Rect(0,0,200f-pad*4f,ObjLibManager.libItemEquipment.lib.Count * lineHeight)
        );

        // Populate Equipment Scroll View.
        int idx = 0;
        foreach (string key in ObjLibManager.libItemEquipment.lib.Keys)  {
            if (GUI.Button(new Rect(pad, (lineHeight + 5f) * idx, 200f-pad*4f,lineHeight), key)) {
                //Debug.Log(key);
                pc.im.addItem(ObjLibManager.libItemEquipment.GetCopy<ItemInventory>(key));
            }
            idx++;
        }

        // End Equipment Scroll View.
        GUI.EndScrollView();



        /* * * * * * * * * *
         * INVENTORY SECTION (ON THE RIGHT)
         * * * * * * * * * */
        // Header for Inventory Section.
        GUI.Label(new Rect(screenWidth/2f + pad/2f,lineHeight*2,screenWidth/2f - pad,24f),"Inventory");

        // Inventory View Label.
        float colWidth = 200f;
        List<ItemContainer> containers = pc.im.getContainers();
        for (int i = 0; i < containers.Count; i++) {
            foreach(int key in containers[i].data.items.Keys) {
                if (containers[i].data.items[key] == null) {
                    GUI.Label(
                        new Rect(screenWidth/2f + pad/2f + pad * 2 + (colWidth * i), lineHeight*(3+key),200f,24f),
                        "NONE"
                    );
                }
                else {
                    GUI.Label(
                        new Rect(screenWidth/2f + pad/2f + pad * 2 + (colWidth * i), lineHeight*(3+key),200f,24f),
                        containers[i].data.items[key].GetComponent<ItemInventory>().data.displayName
                    );
                }
            }
        }

        // Item Scroll View Label.
        GUI.Label(new Rect(screenWidth/2f + pad/2f,currentLineY,screenWidth/2f - pad/2f,24f),"Item Library:");

        // Scroll View of all Items.
        Vector2 itemScrollView = Vector2.zero;
        itemScrollView = GUI.BeginScrollView(
            new Rect(screenWidth/2f + pad/2f, currentLineY + lineHeight * 2, 200f, 300f),
            itemScrollView,
            new Rect(0f,0f,200f-pad*4f,ObjLibManager.libItemInventory.lib.Count * lineHeight)
        );

        // Populate Item Scroll View.
        idx=0;
        foreach (string key in ObjLibManager.libItemInventory.lib.Keys) {
            if (GUI.Button(new Rect(pad, (lineHeight + 5f) * idx, 200f-pad*4f,lineHeight), key)) {
                pc.im.addItem(ObjLibManager.libItemInventory.GetCopy<ItemInventory>(key));
            }
            idx++;
        }

        // End Inventory Scroll View.
        GUI.EndScrollView();
    }
}
