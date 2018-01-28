#conding=utf-8

def fuc_name():
    pass;
    print("name");
def fuc_ab(a,b):
    c=a+b;
    print(c);

def fuc1():
    c=6+66;
    return c;
def fu2(a,b):
    print("fu2");
    if isinstance(a,int) and isinstance(b,int):  #判断a的类型
        return a+b;
    else:
        return "参数类型错误";

#断言判断，返回bool
print(fu2(1,2)==3);

fuc_name();
fuc_ab(5,6);
print(fuc1());