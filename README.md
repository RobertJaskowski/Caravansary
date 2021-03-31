
![GitHub release (latest by date)](https://img.shields.io/github/v/release/robertjaskowski/Caravansary?color=%23&style=for-the-badge)
![GitHub Release Date](https://img.shields.io/github/release-date/robertjaskowski/caravansary?style=for-the-badge)
<br>
![Twitter Follow](https://img.shields.io/twitter/follow/rjjaskowski?label=Developer&style=social)

## **Caravansary** is productivity tools always visible on desktop. There is no need for window switching or managing multiple processes for different tasks.

### Features 
- Click through by default (always stays on top of windows and is not clickable until Ctrl is pressed)
- Plugable architecture ( Develop easily new modules and inject them into the app )
- [Timer that is active when working with process blacklist](https://github.com/RobertJaskowski/ActiveTimer)
- [Colored bar state (used with other plugins)](https://github.com/RobertJaskowski/MainBar)
- [Daily goal tracker](https://github.com/RobertJaskowski/DailyGoal)
- [Key counter](https://github.com/RobertJaskowski/KeyCounter)
- [Road map / todo](https://github.com/RobertJaskowski/RoadMap)


### Contribute
1. Star this repo
2. Suggest features, modules 
3. or help developing it
   - Clone this repo
   - Create wpf class library project
   - Reference Caravansary.SDK
   - Implement ICoreModule
   - Modules in folder 'Modules' will be automatically detected and run in the app
   - Pack dll and extra files to .zip
   - Create a task or edit OnlineModuleList, so your module can be visible in the application module list.
4. Thank you <3
