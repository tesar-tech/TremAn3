{\rtf1\ansi\ansicpg1250\cocoartf1561\cocoasubrtf400
{\fonttbl\f0\fswiss\fcharset0 Helvetica;\f1\fnil\fcharset0 Menlo-Bold;\f2\fnil\fcharset0 Menlo-Regular;
\f3\fmodern\fcharset0 Courier;}
{\colortbl;\red255\green255\blue255;\red34\green34\blue34;\red160\green32\blue240;\red34\green139\blue34;
\red0\green0\blue255;\red34\green34\blue34;}
{\*\expandedcolortbl;;\csgenericrgb\c13333\c13333\c13333;\csgenericrgb\c62745\c12549\c94118;\csgenericrgb\c13333\c54510\c13333;
\csgenericrgb\c0\c0\c100000;\csgenericrgb\c13333\c13333\c13333;}
\paperw11900\paperh16840\margl1440\margr1440\vieww14740\viewh10360\viewkind0
\pard\tx566\tx1133\tx1700\tx2267\tx2834\tx3401\tx3968\tx4535\tx5102\tx5669\tx6236\tx6803\pardirnatural\partightenfactor0

\f0\b\fs28 \cf0 ComTests.cs\
\
\pard\pardeftab720\partightenfactor0

\f1\fs24 \cf2 GetComFromCurrentFrames_DifferentFrames_SameResult()
\f2\b0 \
	This test is performed with frames from a chessboard video\
Ethalon - Matlab code (Matlab has different indexing than C#)\
\
\pard\pardeftab720\fi719\partightenfactor0

\f3\fs20 \cf0 v = VideoReader(\cf3 'test_video.avi'\cf0 );
\fs24 \

\fs20 fs = v.FrameRate;
\fs24 \

\fs20 \cf4 %reduce size for faster computation 
\fs24 \cf0 \

\fs20 sizeReductionFactor = 1;\cf4 %1 -> same resolution; 0.5 -> half resolution
\fs24 \cf0 \

\fs20 sizeX = round(v.Height*sizeReductionFactor);
\fs24 \

\fs20 sizeY= round(v.Width*sizeReductionFactor);
\fs24 \

\fs20 \cf4 %variable for video in gray
\fs24 \cf0 \

\fs20 V3 = zeros([sizeX sizeY   round(v.FrameRate * v.Duration)]);
\fs24 \

\fs20 ii = 1;\cf4 %frame iteration
\fs24 \cf0 \

\fs20 \cf5 while\cf0  hasFrame(v)
\fs24 \

\fs20     \cf4 %change size and convert to gray
\fs24 \cf0 \

\fs20     V3(:,:,ii) = mat2gray(rgb2gray( imresize(readFrame(v),[sizeX sizeY])));
\fs24 \

\fs20      ii = ii+1;
\fs24 \

\fs20 \cf4 %     ii
\fs24 \cf0 \

\fs20 \cf5 end
\fs24 \cf0 \

\fs20 \cf4 %%
\fs24 \cf0 \

\fs20 \cf4 %V3 = zeros(256,256,2);
\fs24 \cf0 \

\fs20 V3Diff = diff(V3,1,3);\cf4 %difference of consequent frames -> motion detection
\fs24 \cf0 \

\fs20 \cf4  
\fs24 \cf0 \

\fs20 \cf4 % tic
\fs24 \cf0 \

\fs20 x_centers = zeros(1,size(V3Diff,3));
\fs24 \

\fs20 y_centers = zeros(1,size(V3Diff,3));
\fs24 \

\fs20  
\fs24 \

\fs20 V4_e = zeros([size(V3Diff,1),size(V3Diff,2),3,length(V3Diff)]);\cf4 %for showing currnet centerOfMass
\fs24 \cf0 \

\fs20 centerOfMotionX = size(V3Diff,2) /2;\cf4 %kdyby byl 1. diff 0 (stejny prvni a druhy frame)
\fs24 \cf0 \

\fs20 centerOfMotionY = size(V3Diff,1) /2;
\fs24 \

\fs20  
\fs24 \

\fs20 x = 1 : size(V3Diff, 2); 
\fs24 \

\fs20 y = 1 : size(V3Diff, 1); 
\fs24 \

\fs20 [X, Y] = meshgrid(x, y);
\fs24 \

\fs20 \cf5 for\cf0  ii = 1:size(V3Diff,3)\cf4 %for every frame
\fs24 \cf0 \

\fs20     
\fs24 \

\fs20     \cf4 % A = imbinarize(abs(V4D(:,:,ii)));
\fs24 \cf0 \

\fs20     A = mat2gray(V3Diff(:,:,ii));\cf4 %this is important part that creates something
\fs24 \cf0 \

\fs20     \cf4 %different than centerOfMass, but works great
\fs24 \cf0 \

\fs20     
\fs24 \

\fs20     
\fs24 \

\fs20     meanA = mean(A(:));
\fs24 \

\fs20     \cf5 if\cf0 (meanA~=0)\cf4 % In case of same frames is diff 0 -> centerOfMotion will be copied from previous frame
\fs24 \cf0 \

\fs20         centerOfMotionX = mean(A(:) .* X(:)) / meanA;
\fs24 \

\fs20         centerOfMotionY = mean(A(:) .* Y(:)) / meanA;
\fs24 \

\fs20     \cf5 end
\fs24 \cf0 \

\fs20     
\fs24 \

\fs20     x_centers(ii) = centerOfMotionX;
\fs24 \

\fs20     y_centers(ii) = centerOfMotionY;
\fs24 \

\fs20     
\fs24 \

\fs20     
\fs24 \

\fs20     \cf5 if\cf0  false \cf4 %create video with point in com. (slows computation)
\fs24 \cf0 \

\fs20         V4_e_current = cat(3,V3Diff(:,:,ii),V3Diff(:,:,ii),V3(:,:,ii));
\fs24 \

\fs20         \cf5 if\cf0  (~isnan(centerOfMotionX) && ~isnan(centerOfMotionY))
\fs24 \

\fs20             V4_e_current =insertShape( V4_e_current,\cf3 'FilledCircle'\cf0 ,[centerOfMotionX,centerOfMotionY,8]);
\fs24 \

\fs20         \cf5 end
\fs24 \cf0 \

\fs20         V4_e(:,:,:,ii) = V4_e_current ;
\fs24 \

\fs20     \cf5 end
\fs24 \cf0 \

\fs20 \cf5 end
\fs24 \cf0 \

\fs20 \cf4 % toc
\fs24 \cf0 \

\fs20 vectors = [x_centers', y_centers'];\
\
\pard\pardeftab720\partightenfactor0

\f1\b\fs24 \cf2 GetComFromCurrentFrames_SameFrames_SameResult()
\f2\b0 \uc0\u8232 
\f3 \cf0 	
\f2 \cf6 This test is performed with same frames(full of zeros)
\f3 \cf0 \
\pard\pardeftab720\partightenfactor0

\f2 \cf6 Ethalon - Matlab code (Matlab has different indexing than C#)\
\pard\tx0\pardeftab720\fi690\partightenfactor0

\f3\fs20 \cf0 clear \cf3 all\cf0 ;close \cf3 all\cf0 ; clc;
\fs24 \

\fs20 V3 = zeros(256,256,2);
\fs24 \

\fs20 \cf4 %V3(:,:,:) = 1;
\fs24 \cf0 \

\fs20 V3Diff = diff(V3,1,3);\cf4 %difference of consequent frames -> motion detection
\fs24 \cf0 \

\fs20 \cf4  
\fs24 \cf0 \

\fs20 \cf4 % tic
\fs24 \cf0 \

\fs20 x_centers = zeros(1,size(V3Diff,3));
\fs24 \

\fs20 y_centers = zeros(1,size(V3Diff,3));
\fs24 \

\fs20  
\fs24 \

\fs20 V4_e = zeros([size(V3Diff,1),size(V3Diff,2),3,length(V3Diff)]);\cf4 %for showing currnet centerOfMass
\fs24 \cf0 \

\fs20 centerOfMotionX = size(V3Diff,2) /2;\cf4 %kdyby byl 1. diff 0 (stejny prvni a druhy frame)
\fs24 \cf0 \

\fs20 centerOfMotionY = size(V3Diff,1) /2;
\fs24 \

\fs20  
\fs24 \

\fs20 x = 1 : size(V3Diff, 2); 
\fs24 \

\fs20 y = 1 : size(V3Diff, 1); 
\fs24 \

\fs20 [X, Y] = meshgrid(x, y);
\fs24 \

\fs20 \cf5 for\cf0  ii = 1:size(V3Diff,3)\cf4 %for every frame
\fs24 \cf0 \

\fs20     
\fs24 \

\fs20     \cf4 % A = imbinarize(abs(V4D(:,:,ii)));
\fs24 \cf0 \

\fs20     A = mat2gray(V3Diff(:,:,ii));\cf4 %this is important part that creates something
\fs24 \cf0 \

\fs20     \cf4 %different than centerOfMass, but works great
\fs24 \cf0 \

\fs20     
\fs24 \

\fs20     
\fs24 \

\fs20     meanA = mean(A(:));
\fs24 \

\fs20     \cf5 if\cf0 (meanA~=0)\cf4 % In case of same frames is diff 0 -> centerOfMotion will be copied from previous frame
\fs24 \cf0 \

\fs20         centerOfMotionX = mean(A(:) .* X(:)) / meanA;
\fs24 \

\fs20         centerOfMotionY = mean(A(:) .* Y(:)) / meanA;
\fs24 \

\fs20     \cf5 end
\fs24 \cf0 \

\fs20     
\fs24 \

\fs20     x_centers(ii) = centerOfMotionX;
\fs24 \

\fs20     y_centers(ii) = centerOfMotionY;
\fs24 \

\fs20     
\fs24 \

\fs20     
\fs24 \

\fs20     \cf5 if\cf0  false \cf4 %create video with point in com. (slows computation)
\fs24 \cf0 \

\fs20         V4_e_current = cat(3,V3Diff(:,:,ii),V3Diff(:,:,ii),V3(:,:,ii));
\fs24 \

\fs20         \cf5 if\cf0  (~isnan(centerOfMotionX) && ~isnan(centerOfMotionY))
\fs24 \

\fs20             V4_e_current =insertShape( V4_e_current,\cf3 'FilledCircle'\cf0 ,[centerOfMotionX,centerOfMotionY,8]);
\fs24 \

\fs20         \cf5 end
\fs24 \cf0 \

\fs20         V4_e(:,:,:,ii) = V4_e_current ;
\fs24 \

\fs20     \cf5 end
\fs24 \cf0 \

\fs20     
\fs24 \

\fs20 \cf5 end
\fs24 \cf0 \

\fs20 vectors = [x_centers', y_centers'];
\fs24 \
\pard\pardeftab720\partightenfactor0

\f2 \cf2 \uc0\u8232 }