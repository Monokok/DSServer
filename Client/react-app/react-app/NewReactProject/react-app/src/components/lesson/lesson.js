import "./Style.css";
import { Modal } from "antd";
import React, { useEffect, useCallback } from "react";
import "./Style.css";
import { Table } from "antd";
// import { tab } from "@testing-library/user-event/dist/tab";
import { Button } from "antd/es/radio";

const Lesson = ({ user, setUser, lessons, setLessons, removeLesson }) => {
  // const {  Space, Table, Tag  } = antd;

  /*Столбцы таблицы с занятиями для UserRole "user" (Студент) */
  const columns = [
    {
      title: "Дата",
      dataIndex: "stringDate",
      key: "stringDate",
      sorter: (a, b) => new Date(a.date) - new Date(b.date),
    },
    {
      title: "Преподаватель | Электронная почта",
      dataIndex: "teacherName",
      key: "teacherName",
    },
    {
      title: "Номер",
      dataIndex: "teacherPhoneNumber",
      key: "teacherPhoneNumber",
      sorter: (a, b) => new Date(a.date) - new Date(b.date),
    },
    {
      title: "Категория",
      dataIndex: "cathegory_id",
      key: "cathegory_id",
      render: (text, row) => (
        <div>
          {(row.cathegory_id === 0 && <div>A</div>) ||
            (row.cathegory_id === 1 && <div>B</div>) ||
            (row.cathegory_id === 2 && <div>C</div>)}
        </div>
      ),
    },
    {
      title: "Статус",
      dataIndex: "type_id",
      key: "type_id",
      sorter: (a, b) => a.type_id - b.type_id,

      render: (text, row) => (
        <div>
          {(row.type_id === 0 && <div>Назначено</div>) ||
            (row.type_id === 1 && <div>Отменено</div>) ||
            (row.type_id === 2 && <div>Проведено</div>)}
        </div>
      ),
    },
    {
      title: "Действие",
      dataIndex: "id",
      key: "id",
      render: (text, row) => (
        <div>
          {row.type_id === 0 && (
            <div>
              <li>
                <Button
                  onClick={() => {
                    CancelLesson(row);
                    console.log("Row data = ", row);
                  }}
                  type="text"
                >
                  Отменить
                </Button>
              </li>
            </div>
          )}
        </div>
      ),
    },
  ];

  /*Столбцы таблицы с занятиями для UserRole "teacher" (Преподаватель) */
  const TeacherColumns = [
    {
      title: "Дата",
      dataIndex: "stringDate",
      key: "stringDate",
      sorter: (a, b) => new Date(a.date) - new Date(b.date),
    },
    {
      title: "ФИО обучающегося",
      dataIndex: "studentName",
      key: "studentName",
    },
    {
      title: "Номер",
      dataIndex: "studentPhoneNumber",
      key: "studentPhoneNumber",
      sorter: (a, b) => new Date(a.date) - new Date(b.date),
    },
    {
      title: "Категория",
      dataIndex: "cathegory_id",
      key: "cathegory_id",
      render: (text, row) => (
        <div>
          {(row.cathegory_id === 0 && <div>A</div>) ||
            (row.cathegory_id === 1 && <div>B</div>) ||
            (row.cathegory_id === 2 && <div>C</div>)}
        </div>
      ),
    },
    {
      title: "Статус",
      dataIndex: "type_id",
      key: "type_id",
      sorter: (a, b) => a.type_id - b.type_id,

      render: (text, row) => (
        <div>
          {(row.type_id === 0 && <div>Назначено</div>) ||
            (row.type_id === 1 && <div>Отменено</div>) ||
            (row.type_id === 2 && <div>Проведено</div>)}
        </div>
      ),
    },
    {
      title: "Действие",
      dataIndex: "id",
      key: "id",
      render: (text, row) => (
        <div>
          {row.type_id === 0 && (
            <div>
              <li>
                <Button
                  onClick={() => {
                    CancelLesson(row);
                    console.log("CancelLesson row data = ", row);
                  }}
                  type="text"
                >
                  Отменить запись
                </Button>
              </li>
            </div>
          )}
          {/* <li>
            <Button
              onClick={() => {
                deleteTableItem(row);
                console.log("deleteTableItem row data = ", row);
              }}
              type="text"
            >
              Удалить запись из БД
            </Button>
          </li> */}
        </div>
      ),
    },
  ];

  //функция получения данных о занятиях от Сервера
  //useCallback - если значения зависимостей не изменились между рендерами компонента,
  //useCallback() вернет сохраненную версию функции, что поможет избежать лишних перерисовок компонента и повторных вычислений функции.
  const getLessons = useCallback(async () => {
    if (user === undefined || user.id === undefined)
      return console.log("getLessons(): user is undefined:", user);
    else console.log("getLessons(): user is OK:", user);
    //функция обратного вызова, которая вызывается React в подходящее время;
    const requestOptions = {
      method: "GET",
    };
    try {
      return await fetch(`api/lessons/${user.id}`, requestOptions)
        .then((response) => response.json())
        .then(
          (data) => {
            // data - данные от сервера
            console.log("Lessons:", data);
            //console.log("setLessons:", setLessons);

            setLessons(data); // Изменение состояния списка занятий
          },
          (error) => {
            console.log(error);
          }
        );
    } catch (error) {
      console.log("Возникла ошибка метода getLessons()!");
      console.log(error);
      throw Object.assign(new Error("Нет данных от Сервера!"), { code: 402 });
    }
  }, [setLessons, user]);

  // useEffect для получения занятий при монтировании компонента или изменении user.id
  useEffect(() => {
    getLessons();
  }, [getLessons]); // При изменении user.id вызывается getLessons()

  /*Функция удаления занятия из таблицы Преподавателя*/
  const deleteTableItem = async ({ id }) => {
    try {
      console.log("Delete TableId:", id);
      const requestOptions = { method: "DELETE" };
      await fetch(`api/Lessons/${id}`, requestOptions).then(
        (response) => {
          if (response.ok) {
            removeLesson(id);
            Modal.success({
              title: "Занятие удалено",
              content: "Удаление занятия успешно!",
            });
          } else {
            Modal.error({
              title: "Занятие не удалено",
              content: "Удаление занятия неуспешно!",
            });
            throw Object.assign(new Error("Ошибка удаления занятия!"));
          }
        },
        (error) => console.log(error)
      );
      return;
    } catch {}
  };

  /*Ф-ция обновления занятия - для его отмены */
  const CancelLesson = async (lesson) => {
    const lessonId = lesson.id;
    console.log("CancelLesson(): lesson = ", lesson, lessonId);
    const typeId = 1;
    const requestOptions = {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ lessonId, typeId }), //lessonId и type как один объект
    };
    try {
      const response = await fetch(
        `api/lessons/${lessonId}/${typeId}`,
        requestOptions
      );
      if (!response.ok) {
        throw new Error("Ошибка при обновлении урока");
      } else {
        switch (response.status) {
          case 200:
            Modal.success({
              title: "Успешная отмена занятия",
              content: "Занятие успешно отменено!",
            });

            getLessons();
            break;

          default:
            break;
        }
      }
    } catch (error) {
      console.error("Ошибка при обновлении урока:", error);
      //модальное окно с сообщением об ошибке
      Modal.error({
        title: "Ошибка",
        content:
          "Ошибка при обновлении урока. Пожалуйста, попробуйте снова позже.",
      });
    }
  };

  /*Блок, который отображается для UserRole = "Пользователь" */
  if (user.isAuthenticated) {
    console.log("User:", user);

    if (user.userRole === "user")
      return (
        <React.Fragment>
          <h3>Список всех занятий:</h3>
          <Table
            dataSource={lessons}
            columns={columns}
            rowKey={(lessons) => lessons.id}
          ></Table>
        </React.Fragment>
      );
    else if (user.userRole === "teacher")
      /*Блок, который отображается для UserRole = "Преподаватель" */
      return (
        <React.Fragment>
          <h3>Список всех занятий:</h3>
          <Table
            dataSource={lessons}
            columns={TeacherColumns}
            rowKey={(lessons) => lessons.id}
          ></Table>
        </React.Fragment>
      );
  } else
  /*Блок, который отображается для неавторизованных пользователей */
    return <h4>Чтобы узнать о предстоящих занятиях, войдите в аккаунт!</h4>;
  //} else if (user.userRole === "teacher") return <div>А ты препод!</div>
};

export default Lesson;
