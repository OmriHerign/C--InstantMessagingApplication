This is a C# .NET 4.0 TCP Client/Server Instant Messaging Application Made by Omri Hering

How to use:

1. Run the tcp server, 
   if it is not you firs time you user is saved 
   on local DB and load with server start-up
2.Run clients as much as you want in each client (on localhost).
	a.Register to the chat if it is your firs time and login
	b.If it is not you first time log-in.
	c.Write down the user name you want to talk with and click add.
	d.The user will appeare in the list of Known users.
	e.Select the relevant user and press connect.
	f.A session window will open, Start chating when title notify that user is available.
	g.Write your message and press Send button.
	h.When conversation ends press the close button on the top of the converation.
	i.To close chat program click logout in the login form.

Notes:
	1. Do not start the client before the server.
	2. You cant connect and reconnect to a session.
	3. Log file of the conversation will be save in localdirectory\bin\debug\Logs
	4. The file name will be the date of the converastion with the name of the participants.
	5. If this file allready exits the converation will be append to the end of the file.
	6. If Logs folder size is more that 10MB the oldest file will be deleted.
	7. If member in converstion leaves the chat the other participant will be notice. 