function [Power, Frequency] = Welch(signal1, signal2, Window, overlap, fs)
%this output same data as pwelch(signal1,Window,overlap,length(Window),fs)
%or cpsd(signal1,signal2,Window,overlap,length(Window),fs)
% inspirated by https://www.mathworks.com/matlabcentral/fileexchange/53464-welch-s-cross-power-spectral-density

    l              = numel(signal1);
    Window = Window(:);signal1 = signal1(:);signal2 = signal2(:);
    WindowCompensation      = sum(Window.* Window);
    SegmentLength           = length(Window);
    SegmentIndices          = (1 : SegmentLength);
    FftLength               = 2^nextpow2(SegmentLength);
    Power                   = 0;
    SegmentCounter          = 0;
    isCpsd = ~isempty(signal2);
    
    while true
                          
        if ~isCpsd %the only difference between cpsd and welch
            Segment1      	= fft(signal1(SegmentIndices) .* Window, FftLength);
            Power           = Power + Segment1 .* conj(Segment1);
        else
            Segment1    	= fft(signal1(SegmentIndices) .* Window, FftLength);
            Segment2       	= fft(signal2(SegmentIndices) .* Window, FftLength);
            Power           = Power + Segment1 .* conj(Segment2);
        end
        
        SegmentCounter    	= SegmentCounter + 1;
        SegmentIndices     	= SegmentIndices + (SegmentLength - overlap);
                
        if SegmentIndices(end) > l
            break
        end
        
    end
    
    % Parseval's Law: PSD / df = PSD * N / fs
    Power                   = Power / (SegmentCounter * WindowCompensation);
    Power                   = Power(1 : end/2 + 1) / fs;
    Power(2 : end - 1)  	= Power(2 : end - 1) * 2;
    Frequency             	= fs / 2 * linspace(0, 1, FftLength / 2 + 1).';
                                   
end