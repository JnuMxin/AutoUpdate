cd ..\..\..\..\
mkdir download\newVersionAutoUpdate
xcopy .\newVersionAutoUpdate download\newVersionAutoUpdate /e
rd /s /q AutoUpdate
ping 1.1.1.1 -n 1 -w 30000 > nul