# untitled-multiplayer-game
An untitled 2D multiplayer game built in Unity. Same build is used both for client and server. To run as server, start it with `-nographics`.

## 🏃‍➡️ How to run as server

### Linux
Start:
`./untitled-multiplayer-game.x86_64 -batchmode -nographics -logfile server.log`
</br>
</br>
Stop:
`Ctrl+C`

### Windows
Start:
`./untitled-multiplayer-game.exe -batchmode -nographics -logfile server.log`
</br>
</br>
Stop:
`Get-Process | ? -Property Name -Like '*untitled*' | Stop-Process`