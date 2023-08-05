### 第0天：Python基础语法使用

#### 1. 打印Hello World

```python
print("Hello World")  # 输出: Hello World
```

- **易错点**：忘记括号或引号。
- **小技巧**：使用单引号或双引号都可以。

#### 2. 变量赋值

```python
x = 5
y = "Python"
print(x, y)  # 输出: 5 Python
```

- **易错点**：变量名不要使用保留字，如`if`、`else`等。
- **小技巧**：变量名应有意义，避免使用单个字母（除非是循环计数等）。

#### 3. 数据类型

```python
x = 5           # int
y = 5.5         # float
z = "Python"    # str
print(type(x), type(y), type(z))  # 输出: <class 'int'> <class 'float'> <class 'str'>
```

- **易错点**：混淆不同数据类型，如在字符串和数字之间。
- **小技巧**：使用`type()`函数检查变量的数据类型。

#### 4. 列表

```python
my_list = [1, 2, 3]
print(my_list[1])  # 输出: 2
```

- **易错点**：索引超出范围。
- **小技巧**：使用负索引从列表末尾开始。

#### 5. 元组

```python
my_tuple = (1, 2, 3)
print(my_tuple[1])  # 输出: 2
```

- **易错点**：尝试修改元组元素（元组是不可变的）。
- **小技巧**：当你不想更改值时，使用元组。

#### 6. 字典

```python
my_dict = {"name": "John", "age": 30}
print(my_dict["name"])  # 输出: John
```

- **易错点**：使用不存在的键。
- **小技巧**：使用`get()`方法安全地获取键的值。

#### 7. 条件语句

```python
x = 5
if x > 3:
    print("Greater")  # 输出: Greater
else:
    print("Smaller")
```

- **易错点**：忘记冒号。
- **小技巧**：使用`elif`添加多个条件。

#### 8. 循环

```python
for i in range(3):
    print(i)  # 输出: 0 1 2
```

- **易错点**：无限循环。
- **小技巧**：使用`break`和`continue`控制循环流程。

#### 9. 函数

```python
def my_function():
    print("Hello from a function")

my_function()  # 输出: Hello from a function
```

- **易错点**：参数不匹配。
- **小技巧**：使用默认参数使函数更灵活。

#### 10. 异常处理

```python
try:
    print(5/0)
except ZeroDivisionError:
    print("Cannot divide by zero")  # 输出: Cannot divide by zero
```

- **易错点**：捕获错误的异常类型。
- **小技巧**：使用`finally`块执行无论如何都要执行的代码。

#### 11. 文件操作

```python
with open('file.txt', 'w') as file:
    file.write("Hello, World!")
```

- **易错点**：忘记关闭文件。
- **小技巧**：使用`with`语句自动关闭文件。

#### 12. 类和对象

```python
class MyClass:
    x = 5

obj = MyClass()
print(obj.x)  # 输出: 5
```

- **易错点**：忘记`self`参数。
- **小技巧**：使用继承重用代码。

#### 13. Lambda函数

```python
x = lambda a: a + 10
print(x(5))  # 输出: 15
```

- **易错点**：复杂的lambda函数。
- **小技巧**：仅在需要短小临时函数时使用lambda。

#### 14. 模块导入

```python
import math
print(math.sqrt(16))  # 输出: 4.0
```

- **易错点**：模块名拼写错误。
- **小技巧**：使用`as`关键字为模块创建别名。

#### 15. 列表推导式

```python
squares = [x**2 for x in range(5)]
print(squares)  # 输出: [0, 1, 4, 9, 16]
```

- **易错点**：复杂的推导式。
- **小技巧**：使用条件表达式过滤元素。

#### 16. 字符串格式化

```python
name = "John"
age = 30
print(f"{name} is {age} years old")  # 输出: John is 30 years old
```

- **易错点**：忘记`f`前缀。
- **小技巧**：使用旧式`%`格式化作为替代。

#### 17. 列表排序

```python
my_list = [3, 1, 4, 1, 5, 9]
my_list.sort()
print(my_list)  # 输出: [1, 1, 3, 4, 5, 9]
```

- **易错点**：尝试排序不可比较的类型。
- **小技巧**：使用`sorted()`函数返回新列表。

#### 18. 集合操作

```python
my_set = {1, 2, 3, 2, 1}
print(my_set)  # 输出: {1, 2, 3}
```

- **易错点**：尝试访问集合元素。
- **小技巧**：使用集合进行数学集合操作。

#### 19. 生成器

```python
def my_gen():
    yield 1
    yield 2

gen = my_gen()
print(next(gen))  # 输出: 1
print(next(gen))  # 输出: 2
```

- **易错点**：过早调用`next()`。
- **小技巧**：使用`for`循环迭代生成器。

#### 20. 装饰器

```python
def my_decorator(func):
    def wrapper():
        print("Something is happening before the function is called.")
        func()
        print("Something is happening after the function is called.")
    return wrapper

@my_decorator
def say_hello():
    print("Hello!")

say_hello()
# 输出:
# Something is happening before the function is called.
# Hello!
# Something is happening after the function is called.
```

- **易错点**：忘记返回内部函数。
- **小技巧**：使用装饰器添加功能，而不更改原始代码。



### 第1天：使用Python爬取简单的网页数据

今天的目标是通过一个简单的案例来学习如何使用Python爬取网页上的数据。我们将从最基础的部分开始，逐步深入。

#### 1. Python基础编程和环境设置

- **安装Python**：请确保您的计算机上已安装Python。如果没有，请访问[Python官方网站](https://www.python.org/downloads/)下载并安装。
- **安装开发工具**：推荐使用IDE如PyCharm或Visual Studio Code来编写代码。

#### 2. 创建一个简单的Python爬虫

- **选择目标网站**：为了简单起见，我们可以选择一个没有复杂JavaScript和登录要求的网站，例如Wikipedia的某个页面。

- **分析网页结构**：使用浏览器的开发者工具来查看网页的HTML结构，找到我们想要爬取的数据所在的标签。

- 编写代码：

  - 导入必要的库，例如`requests`。

  - 使用`requests.get()`方法获取网页的HTML内容。

  - 使用正则表达式或其他方法从HTML中提取所需的数据。

  - 打印或保存数据。

#### 安装`requests`库

用于发送HTTP请求。

```bash
pip install requests
```

#### 安装`BeautifulSoup`库

用于解析HTML和XML文档。

```bash
pip install beautifulsoup4
```

#### 安装`lxml`库

用于解析HTML和XML文档，也是BeautifulSoup的一个可选解析器。

```bash
pip install lxml
```

### 示例1：使用`requests`库爬取网页标题


```python
import requests
from bs4 import BeautifulSoup
    
# 使用requests库发送GET请求
response = requests.get('https://www.python.org/')

# 解析HTML内容
soup = BeautifulSoup(response.text, 'html.parser')

# 找到标题标签并打印
title = soup.find('title')
print(title.text)  # 输出: Welcome to Python.org
```

### 示例2：使用`requests`和正则表达式爬取所有链接

```python
import requests
import re

# 使用requests库发送GET请求
response = requests.get('https://www.python.org/')

# 使用正则表达式找到所有链接
links = re.findall('href="https?://.*?"', response.text)

# 打印所有链接
for link in links:
    print(link)
```

正则表达式：`'href="https?://.*?"'`

- `href="`：这部分确保正则表达式与HTML中的链接属性匹配。在HTML中，链接通常在`href`属性中定义，所以这部分确保我们只查找包含`href`属性的部分。
  
    - `https?`：这部分用于匹配链接的协议。`http`和`https`是两种常见的Web协议。问号`?`表示`s`字符是可选的，所以这部分可以匹配`http`或`https`。
- `://`：这部分确保正则表达式与链接的协议部分匹配。冒号和斜杠是协议和链接地址之间的分隔符。
    - `.*?`：这部分用于匹配链接的主体部分。
      - `.`：匹配除换行符之外的任何字符。
      - `*`：表示前面的字符可以出现零次或多次。所以`.*`会匹配任意数量的任意字符。
      - `?`：在这个上下文中，问号使正则表达式变成非贪婪模式。这意味着它会尽可能少地匹配字符。如果没有问号，正则表达式会尽可能多地匹配字符，可能会导致匹配整个HTML文档。通过添加问号，正则表达式会在找到第一个匹配的双引号`"`时停止匹配，从而正确地匹配每个单独的链接。
    - `"`：这部分确保正则表达式与链接的结束引号匹配。这与开始的`href="`部分相匹配，确保整个链接被正确捕获。
    
    总结：这个正则表达式的目的是匹配HTML中的链接。它查找`href`属性，并捕获`http`或`https`协议，然后捕获链接的主体部分，直到遇到结束引号。通过使用非贪婪模式，它确保每个链接都被单独匹配，而不是整个文档被视为一个长链接。
    
### 示例3：使用`requests`和`lxml`解析器爬取特定类别的内容

```python
import requests
from lxml import html

# 使用requests库发送GET请求
response = requests.get('https://www.python.org/')

# 使用lxml解析HTML内容
tree = html.fromstring(response.content)

# 使用XPath找到特定类别的内容
nav_texts = tree.xpath('//ul[@class="menu"]/li/a/text()')

# 打印内容
for text in nav_texts:
print(text.strip())
```

### 使用BeautifulSoup方式

```python
import requests
from bs4 import BeautifulSoup

# 使用requests库发送GET请求
response = requests.get('https://www.python.org/')

# 解析HTML内容
soup = BeautifulSoup(response.text, 'html.parser')

# 找到特定类别的内容
nav_texts = soup.select('ul.menu li a')

# 打印内容
for text in nav_texts:
print(text.text.strip())
```

### 使用正则表达式方式

```python
import requests
import re
from html import unescape

# 使用requests库发送GET请求
response = requests.get('https://www.python.org/')

# 使用正则表达式找到特定类别的内容
nav_texts = re.findall(r'<a href=".*?"[^>]*>(.*?)</a>', response.text)

# 打印内容
for text in nav_texts:
print(unescape(text.strip()))
```

​		请注意，正则表达式解析HTML可能会更加复杂和脆弱，因为HTML的结构可能非常复杂。如果网页结构发生变化，正则表达式可能需要相应地调整。

#### 3. 实践

- **动手实践**：尝试自己编写代码，爬取所选网页上的某些数据。

- **测试和调试**：运行代码，检查是否能够正确爬取数据。如果遇到问题，请尝试调试。

  ### 示例1：爬取新浪新闻首页的新闻标题

  #### HTML代码

  ```html
  <div class="top_newslist">
      <a href="链接地址">新闻标题</a>
      ...
  </div>
  ```

  #### 抓取思路

  1. 使用`requests`库访问新浪新闻首页。
  2. 使用`BeautifulSoup`解析HTML内容。
  3. 使用CSS选择器`.top_newslist a`找到所有新闻标题的链接。
  4. 遍历链接，打印每个链接的文本内容，即新闻标题。

  ```python
  import requests
  from bs4 import BeautifulSoup
  
  response = requests.get('https://news.sina.com.cn/')
  soup = BeautifulSoup(response.text, 'html.parser')
  titles = soup.select('.top_newslist a')
  
  for title in titles:
      print(title.text)
  ```

  ### 示例2：爬取百度搜索结果的链接

  #### HTML代码

  ```html
  <div class="t">
      <a href="链接地址">搜索结果标题</a>
      ...
  </div>
  ```

  #### 抓取思路

  1. 使用`requests`库访问百度搜索结果页面。
  2. 使用`BeautifulSoup`解析HTML内容。
  3. 使用CSS选择器`.t a`找到所有搜索结果的链接。
  4. 遍历链接，打印每个链接的`href`属性，即链接地址。

  ```python
  import requests
  from bs4 import BeautifulSoup
  
  query = 'Python'
  response = requests.get(f'https://www.baidu.com/s?wd={query}')
  soup = BeautifulSoup(response.text, 'html.parser')
  links = soup.select('.t a')
  
  for link in links:
      print(link['href'])
  ```

  ### 示例3：爬取豆瓣电影Top250的电影名称

  #### HTML代码

  ```html
  <div class="hd">
      <a href="链接地址">
          <span class="title">电影名称</span>
          ...
      </a>
  </div>
  ```

  #### 抓取思路

  1. 使用`requests`库访问豆瓣电影Top250页面。
  2. 使用`BeautifulSoup`解析HTML内容。
  3. 使用CSS选择器`.hd a span.title`找到所有电影名称。
  4. 遍历名称，打印每个名称的文本内容。

  ```python
  import requests
  from bs4 import BeautifulSoup
  
  response = requests.get('https://movie.douban.com/top250')
  soup = BeautifulSoup(response.text, 'html.parser')
  movies = soup.select('.hd a span.title')
  
  for movie in movies:
      print(movie.text)
  ```

  ### 示例4：爬取京东某个商品的评论

  #### JSON数据

  评论数据以JSON格式存储，可以直接从URL获取。

  #### 抓取思路

  1. 使用`requests`库访问京东商品评论的JSON数据链接。
  2. 使用`json`库解析JSON内容。
  3. 从JSON数据中提取评论内容。
  4. 遍历评论，打印每个评论的内容。

  ```python
  import requests
  import json
  
  product_id = '100006487373'  # 示例商品ID
  url = f'https://club.jd.com/comment/productPageComments.action?productId={product_id}&score=0&sortType=5&page=0&pageSize=10'
  response = requests.get(url)
  comments_json = json.loads(response.text)
  comments = comments_json['comments']
  
  for comment in comments:
      print(comment['content'])
  ```

  ### 示例5：爬取天气网某个城市的天气预报

  #### HTML代码

  ```html
  <div class="t">
      <li>
          <h1>日期</h1>
          <p class="wea">天气</p>
          <p class="tem">温度</p>
          ...
      </li>
      ...
  </div>
  ```

  #### 抓取思路

  1. 使用`requests`库访问天气网某个城市的天气预报页面。
  2. 使用`BeautifulSoup`解析HTML内容。
  3. 使用CSS选择器`.t li`找到所有天气预报项。
  4. 遍历每个预报项，提取并打印日期、天气和温度的文本内容。

  ```python
  import requests
  from bs4 import BeautifulSoup
  
  city_code = '101010100'  # 北京城市代码
  url = f'http://www.weather.com.cn/weather/{city_code}.shtml'
  response = requests.get(url)
  soup = BeautifulSoup(response.text, 'html.parser')
  forecasts = soup.select('.t li')
  
  for forecast in forecasts:
      date = forecast.select_one('h1').text
      weather = forecast.select_one('.wea').text
      temperature = forecast.select_one('.tem').text.strip()
      print(f'{date}: {weather}, {temperature}')
  ```

#### 4. 总结

- **反思学习过程**：回顾今天学到的内容，思考如何在未来的项目中应用。
- **准备明天的学习**：预习一下使用BeautifulSoup库进行更复杂的网页数据爬取。