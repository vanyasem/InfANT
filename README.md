# InfANT
**InfANT** is an open-source antivirus written in C#. It's distributed under the MIT license.

The **goal** of this project is to make a free antivirus driven by the community that can be easily changed to work in closed environments.

It has **no** databases included, and at this point, in can be used only as a basis for more complicated projects.

Currently, it has 2 languages: English (main) and Russian. If you want to - you can translate LanguageResources.resx and pull it to my fork.

English version works a bit faster, as it has less text to process.

## 🦄 Build status
Build status for **main** fork: [![Build Status](https://img.shields.io/travis/vanyasem/InfANT/master.svg?style=flat-square)](https://travis-ci.org/vanyasem/InfANT)

Build status for **unstable** fork: [![Build Status](https://img.shields.io/travis/vanyasem/InfANT/unstable.svg?style=flat-square)](https://travis-ci.org/vanyasem/InfANT)

UNSTABLE FORK IS **NOT TESTED** AT ALL OR TESTED PARTIALLY.
USE IT AT **YOUR OWN RISK**.

Unstable builds are not published and are not included in the changelog. *Beware!* Version number of those matches the version number of a stable build they were built on.


## 📖 Documentation
Full documentation will be available on Wiki after 3.0 is released.


## 🔧 Compatibility
It was **not** tested on systems other than Windows (7, 8, 8.1, 10) and it probably will never be.

---

## 🚀 Contributing [![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg?style=flat-square)](http://makeapullrequest.com)
All developers should feel welcome and encouraged to contribute to InfANT, see our [getting started](/Documents/CONTRIBUTING.md) document here to get involved.

If you've never contributed to an OSS project before, here's a [getting started](https://akrabat.com/the-beginners-guide-to-contributing-to-a-github-project/) to get you started.

To contribute a **feature or idea** to IntANT, submit an issue and fill in the template. If the request is approved, you or one of the members of the community can start working on it.

If you find a **bug**, please submit a pull request with a failing test case displaying the bug or create an issue.

Pull requests without adequate testing may be delayed. Please add tests alongside your pull requests.


## 🌓 Progress
Changelog lies [here](http://pastebin.com/8Gfq7Di4).

Overall progress will be published later.


## 📃 Futurelog
 * **3.0** "Light Mode" (easy on resources)
 * **3.0** Optimized scanning on high-end machines
 * **3.0** Database entries
 * **3.0** GUI and libs separated in 2 different packages (so it's easy to merge/fork it into something else)
 * **3.0** Improved UI
 * **3.0** Documentation included
 * An installer (currently it's only portable)
 * Scan every file in archives w/o password
 * Scanning integrated into context menu
 * Real-time protection (detect new drives, detect opened binaries, etc.)
 * Binary analysis
 * ?Start with Windows


---
*Inf* stands for 'Infinity' & *ANT* stands for 'Antivirus'
