# Sgt_Imalas-Oni-Mods-UI

This Repo contains Unity UI for the Oni mods of Sgt_Imalas

TMP doesnt allow direct conversion to ONIs LocText component, so this runs via tmp converters - one of them in this project, the other one in my main mod repository
selecting a gameobject in the hierarchy and running the "Convert TMP" action found under Assets (top bar), will automatically generate a tmp converted prefab inside Assets/UIs/ and will automatically add it to an asset bundle with the name of its original parent gameobject. Using the "Export (All platforms)" action afterwards will export those asset bundles and copy them to the (hardcoded) paths found inside the export script. The mod then has to import these bundles and run the TMP converter tool found in my UtilLibs (See any of my mods that use that for reference, ie. DSS, Rocketry Expanded, CGM, ...).
