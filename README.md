[![Build Status](https://travis-ci.org/vanyasem/InfANT.svg?branch=master)](https://travis-ci.org/vanyasem/InfANT)

# InfANT
###Summary
**InfANT*** is an open-source antivirus project written in C#.
It was designed by 2 Russian students in a few months and is distributed under the MIT license.

Currently, it has 2 languages: English (main) and Russian. If you want to - you can translate LanguageResources.resx and pull it to my fork. 

It has **no** databases included, and at this point, in can be used only as a basis for more complicated projects.

It was **not** tested on systems other than Windows (7, 8, 8.1, 10) and it probably will never be. (Until I get really bored)

###Changelog
The full changelog can be found [HERE](http://bitva-pod-moskvoy.ru/_kaspersky/changelog.txt).

It's **not a final version**, this project is in active development. It has both STABLE (master) and UNSTABLE branches.

UNSTABLE FORK IS **NOT TESTED** AT ALL OR TESTED PARTIALLY.
USE IT AT **YOUR OWN RISK**.

Unstable builds are not published and are not included in the changelog. Version number matches the last stable ones.

###Futurelog
 * **3.0** Create a "light mode"
 * **3.0** Optimize scanning on high-end machines
 * **3.0** Add some databases
 * **3.0** Separate GUI and libs in 2 different packages (so it will be easy to merge it into something else)
 * **3.0** Improved UI
 * **3.0** Some documentation
 * Make an installer (it's currently portable only)
 * Add an ability to scan every file in archives w/o password
 * Integrate it into right-click menu
 * Add real-time protection (detect new drives, detect opened binaries, etc.)
 * Add binary analysis
 * ?Add autostart with Windows
 
###Detailed info
It's good to know that English version works faster, as it has less text to process.

Will provide full documentation after I release a 3.0 version.

---
*Inf stands for "Infinity" and ANT stands for "ANTivirus" (ants work really hard).

