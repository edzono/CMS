   M S B A M L   `   `   `  ����>  :CMS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:CMSCMS  5   	   O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 5   S       CMS.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   #localclr-namespace:CMS     5   	   b ^MaterialDesignThemes.Wpf, Version=3.2.0.1979, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5   
   3system&clr-namespace:System;assembly=mscorlib  6S   -   5          � Title$!  Component Monitoring System��5	   	   $	��428��6-   $	��356��6:    � 
ResizeMode$ NoResize=�6F    � WindowStyle$
 None=�6\    � AllowsTransparency . 
������  � WindowStartupLocation$ CenterScreen=�6�   .��5
      ��  �� CornerRadius$ 10��6   ��5   
   �� 5       �� EndPoint$ 0.5,1&�6"    � MappingMode$ RelativeToBoundingBox=�63    �� 
StartPoint$ 0.5,0&�6W   .��5      � 	 � Color$	 	#FFBDBDFF��6   
 � Offset$
 1Q�61   � 5      $	 	#FF060624��6   � 5      $	 	#FF4C4C78��6   5      ._�5   
   �  � RowDefinitions 5      �� 5      $	��35* �6    �� 5      $
��393* �6     � ColumnDefinitions 5      �� 5      $	��78* �6#   �� 5      $	��11* �6#   .+�5      �� $��61,84,11,45q�6   $��1��6/   .+�5      �� $��
Login Form��6   $
��Left=�6-   $��
20,10,0,10q�6H   $	��Top=�6\   $��Segoe UI Black)�6t   $��24(�6�    J� RenderTransformOrigin$ 0.387,0.175&�6�   ���	#DDFFFEFE6�   �� 5       $
��0,15q�6    �� Orientation$ 
Horizontal=�6+   $��31��6D   .+�5!      '  !MaterialDesignThemes.Wpf.PackIcon     Kind$ Account=�6.   $��Center=�6=   $��5q�6X   
�������6c   �� 5"      -   5         ��Name$ txt_emp��5"      )  #MaterialDesignThemes.Wpf.HintAssist   Hint$ Employee Code��6/   $	��200��6^   $��5,4,5,1q�6j   
�������6�   5#      �� 5$      $
��0,15q�6   $ 
Horizontal=�6+   $��31��6D   $ 0.517,1.258&�6P   .+�5%        $	 Key=�6.   $��Center=�69   $��5q�6T   
�������6_   G� 5&      -   5         G�Name$ txt_pass��5&   "   $ Password��64   $	��200��6^   $��5,4,5,1q�6j   
�������6�   5'      �� 5(      $��15,15q�6   $ 
Horizontal=�6,   $��50��6E   .+�5)      �� -   5         ��Name$ btnlogin��5)      $��Login��6/   $��60,0,0,0q�6?   $��95��6Q   6\   5*      5,      �� 5-      $
��Left=�6   $	��100��6/   $��	85,24,0,0q�6<   $	��Top=�6O   $	��170��6g   $��ezgif.com-gif-maker.png��6s   $��2��6�   )  #MaterialDesignThemes.Wpf.DialogHost  50      -   5         $   MaterialDesignEmbeddedDialogHost#��[  50   Q      DialogMargin$ 20q�6�   $��2��6�   $��2��6�      DialogContent 51      �� 52      $��20q�6!   .+�53      �� Gonna close me?65   �� 54      $ 
Horizontal=�6%   $��Right=�6>   .+�55      ��   MaterialDesignFlatButton#��[ 6%    �� 	IsDefault . $��0 8 8 0q�57   )   $2��,materialDesign:DialogHost.CloseDialogCommand��58   )   ��59   "   ��5:   &   True��6;   .��5;   ;   YES5=      �� 5>      #��[ 6%    �� IsCancel . $��0 8 8 0q�5@   )   $2��,materialDesign:DialogHost.CloseDialogCommand��5A   )   ��5B   "   ��5C   &   	False��6<   .��5D   ;   NO5F      5G      5H      .��5J      �� -   5        $ btnclose��5J         OpenDialogCommand#��Z 6,    0 ,MaterialDesignFloatingActionMiniAccentButton#��[ 6m    � ToolTip$ Close��6�   $��323,2,2,396q�6�   $
��Auto��6  $
��Auto��6  ��6�   Z� ��6�   Z� .��5K        $ Power=�6.   5L      5M      5P      5Q      5T      