TRANSLATE BOT TO ANY LANGUAGE
*****************************
You can do it with any application tha can open .resx files. Like this one: https://github.com/HakanL/resxtranslator
Open It and inside it select the PokemonGo.RocketAPI.Console directory
Add your desired language
Translate all texts in your langaguae row.
Save all changes.
Got it, now you need compile it to optain latest ".dll" file to can use in the bot.


HOW TO COMPILE A SPANISH TRANSLATED RESX FILE.
*************************************
"C:\Program Files\Microsoft SDKs\Windows\v7.1\Bin\ResGen.exe" GUI.es.resx
"C:\Program Files\Microsoft SDKs\Windows\v7.1\Bin\AL.exe" GUI.es.resources /out:PokemonGo.RocketAPI.Console.resources.dll

And copy the result file in a directory called "es" in same place that there is the final .exe file.