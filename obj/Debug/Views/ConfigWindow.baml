   M S B A M L   `   `   `  ����N  JACDCTestSystemPart1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullE'clr-namespace:ACDCTestSystemPart1.ViewsACDCTestSystemPart1.Views  5      ,    &ACDCTestSystemPart1.Views.ConfigWindow   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      _ [MaterialDesignThemes.Wpf, Version=4.8.0.0, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5      3local'clr-namespace:ACDCTestSystemPart1.Views  5      .��5
      � .+�5   
   �� .  (MaterialDesignThemes.Wpf.ColorZoneAssist    Mode$  Inverted=�5      $��18(�5      $��设置��5      .��5      �  � ColumnDefinitions 5      �� 5      �� 5      .+�5      A�  A� Rows$ 10��6   .+�5      ��    LabelTextAllowString#��[  6   
 � Tag$ 扫码枪端口号��6O   ��6\   �� *ConnecteConfig+  c2v �� 	Converter# [  �� ConverterParameter$ HoneyWellPort���� 5      #��[  6   $ 读卡器端口号��6O   ��6\   �� *ConnecteConfig+# [ $ FIDPort���� 5      #��[  6   $ 本机IP地址��6O   ��6\   �� *ConnecteConfig+# [ $ IPAddr���� 5      #��[  6   $ PLC IP地址��6O   ��6^   �� *ConnecteConfig+# [ $ PLCAddr���� 5      #��[  6   $ 数据库名称��6O   ��6[   �� *ConnecteConfig+# [ $ DataBaseString��5      � 5      $��1��6    � RowDefinitions 5      �� 5      $��9* �6(   �� 5      $��* �6(   .+�5       A� $ 9��6"   .+�5!      �� #��[  6"   $ 文件名称��6S   ��6^   �� *
TestConfig+  t2v# [ $ FileName���� 5"      #��[  6"   $ 连续不良次数��6S   ��6`   �� *ConnecteConfig+# [ $ NGCount���� 5#      #��[  6"   $ 负载放电时间��6S   ��6`   �� *
TestConfig+# [ $ ElectrifyTime���� 5$      #��[  6"   $ 负载开机时间��6S   ��6`   �� *
TestConfig+# [ $ StartupTime���� 5%      #��[  6"   $ 负载输出电压上限��6S   ��6b   �� *
TestConfig+# [ $ 
VoltageMax���� 5&      #��[  6"   $ 负载输出电压下限��6S   ��6b   �� *
TestConfig+# [ $ 
VoltageMin��5'      A� 5(      $ 1��6"   $��2��6+   .+�5)      �� $��保存文件��6!   $
��20,0q�60   
���   ���6>   �� *SaveCommand+6^   �� 5*      $��打开文件��6!   $
��20,0q�60   
���   ���6>   �� *OpenCommand+6^   �� 5+      $��退出��6!   $
��20,0q�6.   
���   ���6<   �� *ExitCommand+6\   5,      5-      5.      5/      50      51      