import { Record, Union } from "./fable_modules/fable-library-js.4.24.0/Types.js";
import { list_type, record_type, bool_type, string_type, union_type, class_type } from "./fable_modules/fable-library-js.4.24.0/Reflection.js";
import { newGuid } from "./fable_modules/fable-library-js.4.24.0/Guid.js";
import { ofArray, filter, map, singleton, append } from "./fable_modules/fable-library-js.4.24.0/List.js";
import { createObj, curry2, equals } from "./fable_modules/fable-library-js.4.24.0/Util.js";
import { Cmd_none } from "./fable_modules/Fable.Elmish.4.2.0/cmd.fs.js";
import { createElement } from "react";
import { isNullOrWhiteSpace, printf, toText, join } from "./fable_modules/fable-library-js.4.24.0/String.js";
import { PropHelpers_createOnKey } from "./fable_modules/Feliz.2.9.0/./Properties.fs.js";
import { key_enter } from "./fable_modules/Feliz.2.9.0/Key.fs.js";
import { reactApi } from "./fable_modules/Feliz.2.9.0/./Interop.fs.js";
import { singleton as singleton_1, append as append_1, map as map_1, delay, toList } from "./fable_modules/fable-library-js.4.24.0/Seq.js";

export class TodoId extends Union {
    constructor(Item) {
        super();
        this.tag = 0;
        this.fields = [Item];
    }
    cases() {
        return ["TodoId"];
    }
}

export function TodoId_$reflection() {
    return union_type("TodoComponent.TodoId", [], TodoId, () => [[["Item", class_type("System.Guid")]]]);
}

export function TodoIdModule_unwrap(_arg) {
    const id = _arg.fields[0];
    return id;
}

export class Todo extends Record {
    constructor(Id, Description, Completed) {
        super();
        this.Id = Id;
        this.Description = Description;
        this.Completed = Completed;
    }
}

export function Todo_$reflection() {
    return record_type("TodoComponent.Todo", [], Todo, () => [["Id", TodoId_$reflection()], ["Description", string_type], ["Completed", bool_type]]);
}

export function TodoModule_create(description) {
    return new Todo(new TodoId(newGuid()), description, false);
}

export function TodoModule_toggleCompleted(todo) {
    return new Todo(todo.Id, todo.Description, !todo.Completed);
}

export class State extends Record {
    constructor(Todos, NewTodoDescription) {
        super();
        this.Todos = Todos;
        this.NewTodoDescription = NewTodoDescription;
    }
}

export function State_$reflection() {
    return record_type("TodoComponent.State", [], State, () => [["Todos", list_type(Todo_$reflection())], ["NewTodoDescription", string_type]]);
}

export class Msg extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["NewTodoChanged", "NewTodoSubmitted", "DeleteTodo", "ToggleTodoCompleted"];
    }
}

export function Msg_$reflection() {
    return union_type("TodoComponent.Msg", [], Msg, () => [[["Item", string_type]], [], [["Item", TodoId_$reflection()]], [["Item", TodoId_$reflection()]]]);
}

function StateModule_withNewTodo(description, state) {
    const newTodo = new Todo(new TodoId(newGuid()), description, false);
    return new State(append(state.Todos, singleton(newTodo)), "");
}

function StateModule_withNewTodoDescription(description, state) {
    return new State(state.Todos, description);
}

function StateModule_withTodoCompletedToggle(id, state) {
    const mapToToggle = (toToggleId, todo) => {
        if (equals(todo.Id, toToggleId)) {
            return TodoModule_toggleCompleted(todo);
        }
        else {
            return todo;
        }
    };
    const todos = map(curry2(mapToToggle)(id), state.Todos);
    return new State(todos, state.NewTodoDescription);
}

function StateModule_withoutTodo(id, state) {
    const todos = filter((todo) => !equals(todo.Id, id), state.Todos);
    return new State(todos, state.NewTodoDescription);
}

function StateModule_withNoCommand(state) {
    return [state, Cmd_none()];
}

export function StateModule_init() {
    return [new State(ofArray([TodoModule_create("Buy Egg"), TodoModule_create("Do something")]), ""), Cmd_none()];
}

export function StateModule_update(msg, state) {
    switch (msg.tag) {
        case 1:
            return StateModule_withNoCommand(StateModule_withNewTodo(state.NewTodoDescription, state));
        case 2: {
            const id = msg.fields[0];
            return StateModule_withNoCommand(StateModule_withoutTodo(id, state));
        }
        case 3: {
            const id_1 = msg.fields[0];
            return StateModule_withNoCommand(StateModule_withTodoCompletedToggle(id_1, state));
        }
        default: {
            const description = msg.fields[0];
            return StateModule_withNoCommand(StateModule_withNewTodoDescription(description, state));
        }
    }
}

function View_renderTitle(text) {
    return createElement("p", {
        className: join(" ", ["title"]),
        children: toText(printf("%s"))(text),
    });
}

function View_renderNewTodoInput(currentnewTodo, dispatch) {
    let elems_3, elems, value_3, elems_2, elems_1;
    return createElement("div", createObj(ofArray([["className", join(" ", ["field", "has-addons"])], (elems_3 = [createElement("p", createObj(ofArray([["className", join(" ", ["control", "is-expanded"])], (elems = [createElement("input", createObj(ofArray([["className", join(" ", ["input", "is-medium"])], (value_3 = currentnewTodo, ["ref", (e) => {
        if (!(e == null) && !equals(e.value, value_3)) {
            e.value = value_3;
        }
    }]), ["onChange", (ev) => {
        dispatch(new Msg(0, [ev.target.value]));
    }], ["onKeyUp", (ev_1) => {
        PropHelpers_createOnKey(key_enter, (_arg) => {
            if (!isNullOrWhiteSpace(currentnewTodo)) {
                dispatch(new Msg(1, []));
            }
        }, ev_1);
    }]])))], ["children", reactApi.Children.toArray(Array.from(elems))])]))), createElement("p", createObj(ofArray([["className", join(" ", ["control"])], (elems_2 = [createElement("button", createObj(ofArray([["className", join(" ", ["button", "is-primary", "is-medium"])], ["onClick", (_arg_1) => {
        dispatch(new Msg(1, []));
    }], ["disabled", isNullOrWhiteSpace(currentnewTodo)], (elems_1 = [createElement("i", {
        className: join(" ", ["fas", "fa-plus"]),
    }), "Add"], ["children", reactApi.Children.toArray(Array.from(elems_1))])])))], ["children", reactApi.Children.toArray(Array.from(elems_2))])])))], ["children", reactApi.Children.toArray(Array.from(elems_3))])])));
}

function View_renderTodoList(todos, dispatch) {
    let elems_7;
    return createElement("ul", createObj(singleton((elems_7 = toList(delay(() => map_1((todo) => {
        let elems_6, elems_5, elems, elems_4, elems_3, elems_2;
        return createElement("div", createObj(ofArray([["key", TodoIdModule_unwrap(todo.Id)], ["className", join(" ", ["box"])], (elems_6 = [createElement("div", createObj(ofArray([["className", join(" ", ["columns", "is-mobile"])], (elems_5 = [createElement("div", createObj(ofArray([["className", join(" ", ["column", "is-11"])], (elems = [createElement("p", createObj(toList(delay(() => append_1(todo.Completed ? singleton_1(["style", {
            textDecoration: "line-through",
        }]) : singleton_1(["style", {
            textDecoration: "none",
        }]), delay(() => append_1(singleton_1(["className", join(" ", ["subtitle"])]), delay(() => singleton_1(["children", todo.Description])))))))))], ["children", reactApi.Children.toArray(Array.from(elems))])]))), createElement("div", createObj(ofArray([["className", join(" ", ["column"])], (elems_4 = [createElement("div", createObj(ofArray([["className", join(" ", ["buttons"])], (elems_3 = [createElement("button", createObj(toList(delay(() => append_1(singleton_1(["onClick", (_arg) => {
            dispatch(new Msg(3, [todo.Id]));
        }]), delay(() => append_1(todo.Completed ? singleton_1(["className", join(" ", ["button", "is-success"])]) : singleton_1(["className", join(" ", ["button"])]), delay(() => {
            let elems_1;
            return singleton_1((elems_1 = [createElement("i", {
                className: join(" ", ["fas", "fa-check"]),
            })], ["children", reactApi.Children.toArray(Array.from(elems_1))]));
        })))))))), createElement("button", createObj(ofArray([["className", join(" ", ["button", "is-danger"])], ["onClick", (_arg_1) => {
            dispatch(new Msg(2, [todo.Id]));
        }], (elems_2 = [createElement("i", {
            className: join(" ", ["fas", "fa-trash"]),
        })], ["children", reactApi.Children.toArray(Array.from(elems_2))])])))], ["children", reactApi.Children.toArray(Array.from(elems_3))])])))], ["children", reactApi.Children.toArray(Array.from(elems_4))])])))], ["children", reactApi.Children.toArray(Array.from(elems_5))])])))], ["children", reactApi.Children.toArray(Array.from(elems_6))])])));
    }, todos))), ["children", reactApi.Children.toArray(Array.from(elems_7))]))));
}

export function View_render(state, dispatch) {
    let elems;
    return createElement("div", createObj(ofArray([["style", {
        padding: 20,
    }], (elems = [View_renderTitle("Elmish To-do Ap"), View_renderNewTodoInput(state.NewTodoDescription, dispatch), View_renderTodoList(state.Todos, dispatch)], ["children", reactApi.Children.toArray(Array.from(elems))])])));
}

