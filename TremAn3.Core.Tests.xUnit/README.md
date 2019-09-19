<h3>ComTests.cs </h3>

<b>GetComFromCurrentFrames_DifferentFrames_SameResult()</b> <br />
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

<b>GetComFromCurrentFrames_SameFrames_SameResult()</b> <br />
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

<b>GetMainFreqFromComLists_EnteredValues_SameResult()</b><br />
<p><a href="https://github.com/tesar-tech/treman_algorithms/blob/master/simple_vector_fft.m">Github link</a></p>


<h3>FftTests.cs </h3>

<b>ComputeFftDuringSignal_SinSignal_sameResult()</b> <br />
     This test is performed with sinus signal <br />
Ethalon - Matlab code (Matlab has different indexing than C#) <br />

    f=7;fs=10;
    x1 = sin(2*pi*f*(-15:1/fs:15));
    t = (-10:1/fs:10);
    vec=[];frq=[];
    for i=1:292
        vfftX = fft(x1(i:i+9));
        p1X = abs(vfftX(1:round(length(vfftX)/2)));
        [mx,mx_ind] = max(p1X);
        x_tics =linspace(0,fs/2,length(p1X)) ;
        vec = [vec mx_ind];
        frq = [frq x_tics(mx_ind)];
    end   

<b>ComputeFftDuringSignal_SawToothSignal_sameResult()</b> <br />
     This test is performed with sawtooth signal <br />
Ethalon - Matlab code (Matlab has different indexing than C#) <br />

    f=5;fs=5;
    t = (-10:1/fs:10);
    x1 = 2 * (t - floor(t + 0.5));
    vec=[];frq=[];
    for i=1:94
        vfftX = fft(x1(i:i+7));
        p1X = abs(vfftX(1:round(length(vfftX)/2)));
        [mx,mx_ind] = max(p1X);
        x_tics =linspace(0,fs/2,length(p1X)) ;
        vec = [vec mx_ind];
        frq = [frq x_tics(mx_ind)];
    end


