# 1C-TinyBackup
�������� ������������ ������������� �������������� ��� 1� 

# ��������
��������� ��������� ��������� �������� ����� ���������� �������������� ��� 1�.
������������ ��������� ����������� ����� �������������� ����� main.ini
``` 
[main]
; 0 - silent mode - not show log window and exit after work done
mode=1
; primary backup folder
primary = d:\
; secondary backup folder
secondary = .\

[1]
id=bp
title=�����������
folder=d:\bp
files=30
size=774826665
time=24.12.2015 16:22:27
;filter=*.1cd;*.lgd
;execute=execute.backup
;skip=skip.backup
;compression = 6
;mask=*;*.bat
;exclude=*.cfl;main.cfg
;enabled=1
;recursive=1
;relative=1
;depth=6

[2]
id=ut
title=���������� ���������
folder=d:\ut
filter=1Cv8.1CD
depth=3
```