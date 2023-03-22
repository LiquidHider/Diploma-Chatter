PRINT 'Start: Inserting test data...'

INSERT INTO [ChatUsers]([ID], [LastName], [FirstName], [Patronymic], [UniversityName], [UniversityFaculty], [JoinedUtc], [LastActive], [IsBlocked], [BlockedUntil]) 
VALUES
('46ad9c99-713b-4f9c-8a07-411e3ed9f12c','���������', '���������', '�������������','KhNUE','IT', N'2023-03-17 12:00:00', N'2023-03-21 14:00:00', 0, NULL),
('ae6d7b08-773d-4ea6-802c-d135f69c6b34', '��������', '���', '���������','KhNUE','IT', N'2023-03-18 09:30:15', N'2023-03-23 08:45:30', 0, NULL),
('49766d6d-6b93-4abc-b79c-072948b8943e', '��������', '������ ', NULL,'KhNUE','IT', N'2023-03-19 18:45:00', N'2023-03-22 18:30:15', 1, N'2023-04-12 09:30:00'),
('0fd82ac2-5e91-4554-8e27-752258c2ea72', '��������� ', '������', '³�������','KhNUE','IT', N'2023-03-16 15:20:30', N'2023-03-20 11:20:00',0, NULL),
('791c3a53-22ac-45a9-aa81-b0237e5b82c2', '�������', '�����', NULL,'KhNUE','IT', N'2023-03-20 21:10:45', N'2023-03-24 21:15:45', 1, NULL)

INSERT INTO [Reports]([ID],[ReportedUserID],[Title], [Message])
VALUES
('c4fe5a16-dcc6-4384-8c65-f91d3595bc11','49766d6d-6b93-4abc-b79c-072948b8943e', N'������������ �������', N'���� ������� ��������������� ������������ ������� � ��� � ����������.'),
('e3df4cba-9ab9-47e4-9fc8-b086f7a44d3a','791c3a53-22ac-45a9-aa81-b0237e5b82c2', N'������� �����', N'��� ����� ��������� �������, ����� ������ ��� ����� ����������� ��� ������-������ �� ����������� �������� ��� �������� �� ��� �����.')

INSERT INTO [Identities] ([ID], [Email], [UserTag], [PasswordHash], [PasswordKey], [UserID])
VALUES 
('12fcf73d-895c-487a-a259-606add056881', N'myname1@gmail.com', N'K_Oleks', N'P5rT3qN8mW5xLd9HcF7k9XpJ4V2f', N'vZ5aS1xQ2wE4dR8uT6yU9iO0pM7hF3', '46ad9c99-713b-4f9c-8a07-411e3ed9f12c'),
('5594c258-13d2-4625-ad2d-ee4b42655a51', N'myname2@gmail.com', N'Sh_Yul', N'bN7mK5cL4pJ8zX6vH2dG9fT1qR3sW2', N'yU0iO1pM7hF3nZ5aS4xQ2wE4dR8uT6', 'ae6d7b08-773d-4ea6-802c-d135f69c6b34'),
('d40bc26f-1594-4116-8ce5-9dd7d8c10365', N'myname3@gmail.com', N'P_Max', N'jK5cL4pJ8zX6vH2dG9fT1qR3sW2bN7m', N'rT3qN8mW5xLd9HcF7k9XpJ4V2fgP5aS', '49766d6d-6b93-4abc-b79c-072948b8943e'),
('61744250-51df-4bc0-97b0-7cce4a2bb313', N'myname4@gmail.com', N'Grg_Nat', N'zX6vH2dG9fT1qR3sW2bN7mK5cL4pJ8j', N'0iO1pM7hF3nZ5aS4xQ2wE4dR8uT6yU9', '0fd82ac2-5e91-4554-8e27-752258c2ea72'),
('f9c0fd8b-3125-4efc-9d74-d577b0e17c45', N'myname5@gmail.com', N'Kazik_2008', N'7mK5cL4pJ8zX6vH2dG9fT1qR3sW2bNj', N'5xLd9HcF7k9XpJ4V2fgP5rT3qN8mW6v', '791c3a53-22ac-45a9-aa81-b0237e5b82c2')

INSERT INTO [GroupChats]([ID], [Name], [Description])
VALUES
('722b9a41-ac0a-4460-81ec-f24e09e4904f',N'19.2',N'����� �������� ���� 19.2')

INSERT INTO [UserJoinedGroups]([ID],[GroupID],[UserID], [UserRole])
VALUES
('307acdda-45d6-4c5d-ace2-0168504b7f1b', '722b9a41-ac0a-4460-81ec-f24e09e4904f', '46ad9c99-713b-4f9c-8a07-411e3ed9f12c', 2),
('cbaf92cb-3f66-4dc4-83ff-ace6dab5c7f7', '722b9a41-ac0a-4460-81ec-f24e09e4904f', 'ae6d7b08-773d-4ea6-802c-d135f69c6b34', 1),
('779f2f25-d08b-4836-85fa-88b359b2643b', '722b9a41-ac0a-4460-81ec-f24e09e4904f', '0fd82ac2-5e91-4554-8e27-752258c2ea72', 0)

INSERT INTO [BlockedGroupChatUsers] ([ID],[GroupID],[UserID], [BlockedUntil])
VALUES
('af53ba3a-cbf3-421e-be95-8dbe880343f7', '722b9a41-ac0a-4460-81ec-f24e09e4904f','49766d6d-6b93-4abc-b79c-072948b8943e', N'2023-06-01 00:00:00')


INSERT INTO [Messages]([ID], [Body], [IsEdited], [Sent], [IsRead], [Sender], [RecipientUser], [RecipientGroup])
VALUES
--��������� ��� �� 46ad9c99-713b-4f9c-8a07-411e3ed9f12c �� ae6d7b08-773d-4ea6-802c-d135f69c6b34
('3b6d8357-e98f-4641-a76d-2a521112145e', N'�����', 0, N'2023-03-20 12:25:00', 1, '46ad9c99-713b-4f9c-8a07-411e3ed9f12c', 'ae6d7b08-773d-4ea6-802c-d135f69c6b34', NULL),
('7398159b-53e7-4c82-a24f-0dc84151d19d', N'�����', 0, N'2023-03-20 12:30:00', 1, 'ae6d7b08-773d-4ea6-802c-d135f69c6b34',  '46ad9c99-713b-4f9c-8a07-411e3ed9f12c', NULL),
('40b2cd9d-1aad-4e9c-9d1d-a9616aa15386', N'ϳ������, �� ��� ������ �� ��������� �������?', 1, N'2023-03-20 12:35:00', 1, '46ad9c99-713b-4f9c-8a07-411e3ed9f12c', 'ae6d7b08-773d-4ea6-802c-d135f69c6b34', NULL),
('ab3869d7-7765-4e8d-88c9-8a8517195566', N'�������� ������ ��� �� ���������� ��� �� ���� ����.', 0, N'2023-03-20 12:40:00', 1, 'ae6d7b08-773d-4ea6-802c-d135f69c6b34',  '46ad9c99-713b-4f9c-8a07-411e3ed9f12c', NULL),
('bb42d9ec-0430-472e-9fea-e138d4bd297c', N'�������, �����!', 0, N'2023-03-20 12:45:00', 1, '46ad9c99-713b-4f9c-8a07-411e3ed9f12c', 'ae6d7b08-773d-4ea6-802c-d135f69c6b34', NULL),
--��� ����� 19.2
('601346ad-14a2-4e6e-aad8-3e49720e7fa8', N'����� ���!', 1, N'2023-03-20 13:00:00', 1, '46ad9c99-713b-4f9c-8a07-411e3ed9f12c', NULL, '722b9a41-ac0a-4460-81ec-f24e09e4904f'),
('ed2c5082-9874-44ad-9fb9-46d4ee23de88', N'������!', 0, N'2023-03-20 13:01:00', 0, 'ae6d7b08-773d-4ea6-802c-d135f69c6b34', NULL, '722b9a41-ac0a-4460-81ec-f24e09e4904f'),
('afa1adf4-1b80-41f3-ad72-35b3a0780b59', N'�����!', 0,N'2023-03-20 13:04:00', 0, '49766d6d-6b93-4abc-b79c-072948b8943e', NULL, '722b9a41-ac0a-4460-81ec-f24e09e4904f'),
('eab78826-c728-4b20-a89a-4d3e68edb809', N'�������� �� �� ����! 100 ������� ��� ��� ������� �� ���� bullshit.com!', 0,N'2023-03-20 13:05:00', 0, '791c3a53-22ac-45a9-aa81-b0237e5b82c2', NULL, '722b9a41-ac0a-4460-81ec-f24e09e4904f')

PRINT 'Result: Test data insertion completed.'