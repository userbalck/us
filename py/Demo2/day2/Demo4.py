#conding=utf-8
global arg; #全局变量
arg =1;
def fu_ar():
    global arg;
    arg=5;

def fu_ar2():
    global arg;
    arg=2;
fu_ar();
fu_ar2();
print(arg);