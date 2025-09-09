import React, { useState, useEffect } from 'react'
import { api } from '../api'
import type { User } from '../types'
interface Props {
    onClose(): void
    onSelect(id: number | null, taskID: number, userName: string | null): void
    taskID: number
}
export default function UserListDropDown({ taskID, onClose, onSelect}: Props) {
    const [users, setUsers] = useState<User[]>([])
    const [selectedValue, setSelectedValue] = useState('');
    const [loadingUsers, setLoadingUsers] = useState(true)

    async function load() {
        // Fetch data from API
        setLoadingUsers(true)
        const res = await api.get<User[]>(`/users`, { params: { status: 'active' } })
        setUsers(res.data)
        setLoadingUsers(false)
    }

    useEffect(() => {
        load()
    }, []); 

    const handleDropdownChange = (event) => {
        if (event.target.value == "UnAssign") {
            setSelectedValue('');
            onSelect(null, taskID, null);
        }
        else {
            const userInfo = event.target.value.split('/');
            const userId = userInfo[0];
            const userName = userInfo[1];
            setSelectedValue(userName);
            onSelect(Number(userId), taskID, userName);
        }
    };

    return (
        <div className="dropDownList">
            <div className="dropDownList">
                {loadingUsers ? <p>Loading…</p> :
                    <select value={selectedValue} onChange={e => handleDropdownChange(e)}>
                        <option className="dropDownList" value="">-- Select --</option>
                        <option className="dropDownList" value="UnAssign">-- UnAssign --</option>
                        {users.map(user => (
                            <option className="dropDownList" key={user.id} value={`${user.id} ${'/'} ${user.name}`}>
                                {user.name}
                            </option>
                        ))}
                    </select>}
                <button className="closeButton" onClick={onClose}>Close</button>
            </div>
        </div>
    );
}