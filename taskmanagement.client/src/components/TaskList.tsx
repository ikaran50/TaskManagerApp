import { useMemo, useState, useEffect } from 'react'
import type { TaskItem } from '../types'
import UserListDropDown from '../components/UserListDropDown'
import { api } from '../api'
interface Props {
    tasks: TaskItem[]
    filter: 'all' | 'open' | 'done'
    onToggle(id: number): void
    onDelete(id: number): void
}

export default function TaskList({ tasks, filter, onToggle, onDelete }: Props) {
    const [showUserDropDown, setShowUserDropDown] = useState(true)
    const [currentAssigneeName, setCurrentAssigneeName] = useState('')
    const filteredList = useMemo(() => {
        let list = tasks
        if (filter === 'open') list = tasks.filter(t => !t.isCompleted)
        if (filter === 'done') list = tasks.filter(t => t.isCompleted)
        return list
    }, [tasks, filter])

    useEffect(() => {
        setShowUserDropDown(false);
    }, []); 


    const onClose = () => {
        setShowUserDropDown(false);
    }


    async function onSelect(id: number, taskID: number, userName: string) {
        // update the task with the selected userID
        const payload = {
            UserId: id,
            UserName: userName
        }
        await api.put<TaskItem[]>(`/tasks/${taskID}`, payload)
        if (userName === null) {
            setCurrentAssigneeName('')
        }
        else {
            setCurrentAssigneeName(userName)
        }
    }


    if (!filteredList.length) return <p className="small">No Tasks Added.</p>

    return (
        <div className="list">
            {filteredList.map(t => (
                <div key={t.id} className="card item">
                    <div>
                        <div className="spread">
                            <div style={{ display: 'flex', gap: '0.5rem', alignItems: 'center' }}>
                                <input type="checkbox" checked={t.isCompleted} onChange={() => onToggle(t.id)} />
                                <strong style={{ textDecoration: t.isCompleted ? 'line-through' : 'none' }}>{t.title}</strong>
                            </div>
                        </div>
                        {t.description && <div className="small" style={{ marginTop: 4 }}>{t.description}</div>}
                        <div className="small" style={{ marginTop: 6 }}>
                            {t.dueDate ? `Due: ${new Date(t.dueDate).toLocaleDateString()}` : 'No due date'} • Created {new Date(t.createdAt).toLocaleString()}
                        </div>
                        <p className="small"> Assignee:{currentAssigneeName === '' ? t.userName : currentAssigneeName}</p>
                    </div>
                    <div className="row">
                        <button className="btn" onClick={() => onToggle(t.id)}>{t.isCompleted ? 'Mark Open' : 'Mark Complete'}</button>
                        <button className="btn ghost" onClick={() => onDelete(t.id)}>Delete</button>
                        <button className="btn ghost" onClick={() => setShowUserDropDown(true)}>Assign User</button>
                        {showUserDropDown ? <UserListDropDown taskID={t.id} onClose={onClose} onSelect={onSelect} /> : <></>}
                    </div>
                </div>
            ))}
        </div>
    )
}