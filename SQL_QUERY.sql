CREATE DATABASE ToDoDB;
USE TodoDB;

CREATE TABLE Tasks 
(
    Id INT IDENTITY(1,1) NOT NULL,
    TaskName VARCHAR(30) NOT NULL,
    TaskDetails TEXT NULL,
    CreatedOn DATE DEFAULT(GETDATE()) NOT NULL,
    ModifiedOn DATE NULL,
    CompletedOn DATE NULL,
    Deadline DATE NULL,
    IsCompleted BIT DEFAULT(0),
    Importance INT DEFAULT(2) NOT NULL CHECK (Importance IN (1, 2, 3)),
    Category VARCHAR(30) DEFAULT('HOME') CHECK(Category IN ('HOME', 'OFFICE', 'MARKET')),

    PRIMARY KEY (Id), 
);
GO

INSERT INTO Tasks (TaskName) VALUES ('TEST TASK');

SELECT * FROM Tasks;
UPDATE Tasks SET IsCompleted = 1 WHERE Id = 1;
GO

CREATE TRIGGER update_completedon 
ON Tasks
AFTER UPDATE
AS 
BEGIN
    
    IF (UPDATE (IsCompleted))
    BEGIN
        UPDATE Tasks 
        SET CompletedOn = 
            CASE
                WHEN Tasks.IsCompleted = 0 THEN NULL
                ELSE GETDATE()
            END
        FROM inserted i
        WHERE i.Id = Tasks.Id;
    END
    ELSE 
    BEGIN
        UPDATE Tasks SET ModifiedOn = GETDATE() FROM inserted i WHERE i.Id = Tasks.Id;
    END
END

DROP TRIGGER update_completedon;
UPDATE Tasks SET IsCompleted = ~IsCompleted WHERE Id = 1;
SELECT * FROM Tasks;
UPDATE Tasks SET TaskDetails = 'as,fmbsndf,dsbmsd' WHERE Id = 74;
UPDATE Tasks SET TaskDetails = ' ASD ', TaskName = 'ASFS' WHERE Id = 85;

CREATE TABLE ToDoSettings
(            
    Email VARCHAR(30) NULL,
    MailTime TIME DEFAULT('08:00:00'),
);

SELECT * FROM Tasks;
SELECT * FROM ToDoSettings;
INSERT INTO ToDoSettings VALUES ('nittin1f4@gmail.com', '08:00:00');

SELECT * FROM Tasks WHERE DATEDIFF(day, CompletedOn, GETDATE()) > 0;
--UPDATE Tasks SET CompletedOn = '2023-08-17' WHERE Id = 101;
