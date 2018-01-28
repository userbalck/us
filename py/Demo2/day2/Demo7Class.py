#conding=utf-8

class test(object):
    def get(self,a=None):

        return self.var1;

    def demo1(self):
        print("demo1")
    pass;


    def __init__(self,var1):
        self.var1=var1;


#对象实例化，调用对象
#@实例化空参数
e=test();

#类实例化传参数
t=test("hhhhss");
print(t.get());



