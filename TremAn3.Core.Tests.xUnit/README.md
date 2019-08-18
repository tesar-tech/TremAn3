<b>ComTests.cs_ </b>

GetComFromCurrentFrames_DifferentFrames_SameResult() <br />
     This test is performed with frames from a chessboard video <br />
Ethalon - Matlab code (Matlab has different indexing than C#) <br />

    v = VideoReader('test_video.avi');
    fs = v.FrameRate;
    %reduce size for faster computation 
    sizeReductionFactor = 1;%1 -> same resolution; 0.5 -> half resolution
    sizeX = round(v.Height*sizeReductionFactor);
    sizeY= round(v.Width*sizeReductionFactor);
    %variable for video in gray
    V3 = zeros([sizeX sizeY   round(v.FrameRate * v.Duration)]);
    ii = 1;%frame iteration
    while hasFrame(v)
        %change size and convert to gray
        V3(:,:,ii) = mat2gray(rgb2gray( imresize(readFrame(v),[sizeX sizeY])));
        ii = ii+1;
    %   ii
    end
    %%
    %V3 = zeros(256,256,2);
    V3Diff = diff(V3,1,3);%difference of consequent frames -> motion detection
 
    % tic
    x_centers = zeros(1,size(V3Diff,3));
    y_centers = zeros(1,size(V3Diff,3));
 
    V4_e = zeros([size(V3Diff,1),size(V3Diff,2),3,length(V3Diff)]);%for showing currnet centerOfMass
    centerOfMotionX = size(V3Diff,2) /2;%kdyby byl 1. diff 0 (stejny prvni a druhy frame)
    centerOfMotionY = size(V3Diff,1) /2;
    
    x = 1 : size(V3Diff, 2); 
    y = 1 : size(V3Diff, 1); 
    [X, Y] = meshgrid(x, y);
    for ii = 1:size(V3Diff,3)%for every frame
    
        % A = imbinarize(abs(V4D(:,:,ii)));
        A = mat2gray(V3Diff(:,:,ii));%this is important part that creates something
        %different than centerOfMass, but works great
    
    
        meanA = mean(A(:));
        if(meanA~=0)% In case of same frames is diff 0 -> centerOfMotion will be copied from previous frame
            centerOfMotionX = mean(A(:) .* X(:)) / meanA;
            centerOfMotionY = mean(A(:) .* Y(:)) / meanA;
    end
    
    x_centers(ii) = centerOfMotionX;
    y_centers(ii) = centerOfMotionY;
    
    
        if false %create video with point in com. (slows computation)
            V4_e_current = cat(3,V3Diff(:,:,ii),V3Diff(:,:,ii),V3(:,:,ii));
            if (~isnan(centerOfMotionX) && ~isnan(centerOfMotionY))
                V4_e_current =insertShape( V4_e_current,'FilledCircle',[centerOfMotionX,centerOfMotionY,8]);
            end
            V4_e(:,:,:,ii) = V4_e_current ;
        end
    end
    % toc
    vectors = [x_centers', y_centers'];

GetComFromCurrentFrames_SameFrames_SameResult() <br />
     This test is performed with same frames(full of zeros) <br />
Ethalon - Matlab code (Matlab has different indexing than C#) <br />

    clear all;close all; clc;
    V3 = zeros(256,256,2);
    %V3(:,:,:) = 1;
    V3Diff = diff(V3,1,3);%difference of consequent frames -> motion detection
 
    % tic
    x_centers = zeros(1,size(V3Diff,3));
    y_centers = zeros(1,size(V3Diff,3));
 
    V4_e = zeros([size(V3Diff,1),size(V3Diff,2),3,length(V3Diff)]);%for showing currnet centerOfMass
    centerOfMotionX = size(V3Diff,2) /2;%kdyby byl 1. diff 0 (stejny prvni a druhy frame)
    centerOfMotionY = size(V3Diff,1) /2;
 
    x = 1 : size(V3Diff, 2); 
    y = 1 : size(V3Diff, 1); 
    [X, Y] = meshgrid(x, y);
    for ii = 1:size(V3Diff,3)%for every frame
    
        % A = imbinarize(abs(V4D(:,:,ii)));
        A = mat2gray(V3Diff(:,:,ii));%this is important part that creates something
        %different than centerOfMass, but works great
    
    
        meanA = mean(A(:));
        if(meanA~=0)% In case of same frames is diff 0 -> centerOfMotion will be copied from previous frame
            centerOfMotionX = mean(A(:) .* X(:)) / meanA;
            centerOfMotionY = mean(A(:) .* Y(:)) / meanA;
        end
    
        x_centers(ii) = centerOfMotionX;
        y_centers(ii) = centerOfMotionY;
    
    
        if false %create video with point in com. (slows computation)
            V4_e_current = cat(3,V3Diff(:,:,ii),V3Diff(:,:,ii),V3(:,:,ii));
            if (~isnan(centerOfMotionX) && ~isnan(centerOfMotionY))
                V4_e_current =insertShape( V4_e_current,'FilledCircle',[centerOfMotionX,centerOfMotionY,8]);
            end
            V4_e(:,:,:,ii) = V4_e_current ;
        end
    
    end
    vectors = [x_centers', y_centers'];
