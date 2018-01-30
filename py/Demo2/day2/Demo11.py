#coding=utf-8
'''
提示：需要用到urllib模块
get_httpcode()获取网页的状态码，返回结果例如：200,301,404等 类型为int
get_htmlcontent() 获取网页的内容。返回类型:str
get_linknum()计算网页的链接数目。

'''
#继承

class Base(object):
    def __init__(self,name):
        self.name=name;

class b(Base):
    def get_name(self):
        return self.name;

set_name=b("susu");
print(set_name.get_name())
