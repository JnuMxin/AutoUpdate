cd ..\..\..\..\
mkdir download\newVersionAutoUpdate
xcopy .\AutoUpdate download\newVersionAutoUpdate /e
ping 1.1.1.1 -n 1 -w 30000 > nul