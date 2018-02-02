@echo ChannelSpliter kingchild@163.com
@echo %1
@echo %2
@set pngPath=%1
@set alphaPath=%2
@set toolPath=%3

cd /d %toolPath%
convert %pngPath% -channel Alpha -separate %alphaPath%
convert %pngPath% -channel Alpha -threshold 100%% +channel %pngPath%
@echo Life is short, coding hard.
