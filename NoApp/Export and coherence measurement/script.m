%%
% unzip('data.zip')
%%
% Define paths and movements
clear
movements = {'SForw', 'SRest', 'SWing'};
base_path = 'data';

folder_info = dir(base_path);
folder_info = folder_info([folder_info.isdir]); % Filter out non-directory entries
folder_info = folder_info(~ismember({folder_info.name}, {'.', '..'})); % Remove the '.' and '..' entries

allPatients = struct('Id', {}, 'Folder', {}, 'Results', {});

% Iterate through each folder and extract the ID and name
num_folders = length(folder_info);
for i = 1:num_folders
    
    folder_name = folder_info(i).name;
    disp(strcat("Folder ",num2str(i)," from ",num2str(num_folders)," - ", folder_name))
    match = regexp(folder_name, '^(\d+)_', 'tokens'); % Extract the ID using regex
    if isempty(match)
        warning(strcat(folder_name, " has no id prefix. Skipping this folder"))
        continue
    end
    id = str2double(match{1}{1});

    % Add to the structure array
    allPatients(end+1).Id = id;
    allPatients(end).Folder = folder_name;

    allPatients(end).Results = ComputeCoheForPatient(allPatients(end).Folder,movements,base_path);

end

%% Create Tables with data
[~, sortedIndices] = sort([allPatients.Id]);
allPatients = allPatients(sortedIndices);

pairs = {'HR', 'HL', 'RL'};
numOfDataCols = length(movements) * length(pairs);

% Using cellfun to create combinations for header
combineFunc = @(m, p) strcat(m, '-', p);
combinations = cellfun(combineFunc, reshape(repmat(movements, length(pairs),1),[numOfDataCols,1])', repmat(pairs, 1,length(movements)), 'UniformOutput', false);
headers = [{'Id', 'Folder'}, combinations];

% Define size and variable types
numPatients = length(allPatients);
sz = [numPatients, 2+numOfDataCols ];
varTypes = ["double", "string", repmat("double", 1, numOfDataCols)];

% Initialize the table
maxFreqTable = table('Size', sz, 'VariableTypes', varTypes, 'VariableNames', headers);
maxCoheTable = table('Size', sz, 'VariableTypes', varTypes, 'VariableNames', headers);

% fill the table
for i = 1:numPatients
    row = cell(1,2 +numOfDataCols);
    maxFreqTable(i,1) = {allPatients(i).Id};
    maxCoheTable(i,1) = {allPatients(i).Id};

    maxFreqTable(i,2)= {allPatients(i).Folder};
    maxCoheTable(i,2)= {allPatients(i).Folder};
    indx_col = 3;
    for m = 1:length(movements)
        for p = 1:length(pairs)
            val = allPatients(i).Results.(movements{m}).(pairs{p});
            maxFreqTable(i,indx_col) = {val(1)};
            maxCoheTable(i,indx_col) = {val(2)};
            indx_col= indx_col+1;
        end
    end
end

%% save to csv
writetable(maxFreqTable, 'treman-results_maxFreq.csv');
writetable(maxCoheTable, 'treman-results_maxCohe.csv');

%% Support functions for computing coherence in 2-12 Hz interval

function results = ComputeCoheForPatient(patient,movements,base_path)

% Check if any of the movement folders are missing
if ~all(cellfun(@(x) exist(fullfile(base_path, patient, x), 'dir'), movements))
    disp('One or more movement folders are missing.');
    return;
end


% Loop through each movement
for idx = 1:length(movements)
    movement = movements{idx};

    % Load data from ComX and ComY
    comX = readtable(fullfile(base_path, patient, movement, 'ComX.csv'));
    comY = readtable(fullfile(base_path, patient, movement, 'ComY.csv'));
    info_data = jsondecode(fileread(fullfile(base_path, patient, movement, 'info.json')));
    fs = info_data.FrameRate;


    coheHR = getCohe(comX.H, comY.H, comX.R, comY.R);  % Coherence between H and R in Yellow
    coheHL= getCohe(comX.H, comY.H, comX.L, comY.L);  % Coherence between H and L in Magenta
    coheRL= getCohe(comX.R, comY.R, comX.L, comY.L);  % Coherence between R and L in Cyan

    freqs = linspace(0,fs/2,length(coheHR));
    indexes_interval = [find(freqs > 2, 1, 'first'),find(freqs < 12, 1, 'last') ];

    hr_ind = getMaxIndFromCohe(coheHR,indexes_interval);
    hl_ind = getMaxIndFromCohe(coheHL,indexes_interval);
    rl_ind = getMaxIndFromCohe(coheRL,indexes_interval);

    maxFreq.HR = [freqs(hr_ind),coheHR(hr_ind)];
    maxFreq.HL = [freqs(hl_ind),coheHR(hl_ind)];
    maxFreq.RL = [freqs(rl_ind),coheRL(rl_ind)];

    results.(movement) = maxFreq;
end
end
function cohe= getCohe(comX1, comY1,comX2,comY2)
data1 = sqrt(comX1.^2 + comY1.^2);
data2 = sqrt(comX2.^2 + comY2.^2);

% Compute coherence
cohe = mscohere(data1, data2,hamming(256),255,256);

end

function maxInd = getMaxIndFromCohe(cohe,indexes_interval)
[~,maxInd] = max(cohe(indexes_interval(1):indexes_interval(2)));
% get the correct maximum index
maxInd= maxInd+indexes_interval(1)-1;
end

